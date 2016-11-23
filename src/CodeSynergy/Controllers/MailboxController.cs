using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CodeSynergy.Models;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CodeSynergy.Models.MailboxViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CodeSynergy.Controllers
{
    public class MailboxController : Controller
    {
        private readonly UserRepository _users;
        private readonly PrivateMessageRepository _privateMessages;
        private readonly MailboxRepository _mailboxes;
        private readonly ModerationMailbox _moderationMailbox;
        private readonly ReportRepository _reports;
        private readonly QuestionRepository _questions;

        public MailboxController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users, IRepository<PrivateMessage, long> privateMessages, IJoinTableRepository<UserMailbox, string, byte> mailboxes,
            IRepository<ModerationMailboxItem, int> moderationMailbox, IRepository<Report, int> reports, IRepository<Question, int> questions) : base()
        {
            _users = (UserRepository) users;
            _privateMessages = (PrivateMessageRepository) privateMessages;
            _mailboxes = (MailboxRepository) mailboxes;
            _moderationMailbox = (ModerationMailbox) moderationMailbox;
            _reports = (ReportRepository) reports;
            _questions = (QuestionRepository) questions;
        }

        // Mailbox modal loaded
        // GET: /Mailbox/MailboxTypeID/[MailboxItemID]
        [HttpGet]
        public async Task<IActionResult> Index(int MailboxTypeID, int? MailboxItemID, int PrivateMessageID = 0, string Recipient = null, string returnUrl = null)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            IEnumerable<Mailbox> mailboxes = _mailboxes.GetAllForUser(user);
            PrivateMessage privateMessage = null;
            bool isModerator = user.Role == "Moderator" || user.Role == "Administrator";
            bool hasCachedMessage = false;
            // Add moderation mailbox for moderators and administrators
            if (isModerator)
                mailboxes = mailboxes.Append(_moderationMailbox);
            ViewData["ReturnUrl"] = returnUrl;
            // Add user mailbox unread count
            ViewData["UnreadCount"] = Math.Min(_mailboxes.Find(user.Id, (byte) EnumMailboxType.Inbox).Items.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count(), 99);
            if (isModerator) // Add moderation mailbox unread count for moderators and administrators
                ViewData["ModerationUnreadCount"] = Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99);
            ViewData["ActiveType"] = (byte) MailboxTypeID; // Active mailbox type/tab index

            if (isModerator && PrivateMessageID != 0) // If viewing a private message (moderators/administrators only), set the private message
                privateMessage = _privateMessages.Find(PrivateMessageID);

            // If there is an incomplete message in the user's cookies, retrieve its data and continue writing it
            if ((bool) (ViewData["IsNewMessage"] = ((hasCachedMessage = Request.Cookies.Any(c => c.Key == "MessageModel")) || Recipient != null) && privateMessage == null))
            {
                PrivateMessageViewModel messageModel = hasCachedMessage ? JsonConvert.DeserializeObject<PrivateMessageViewModel>(Request.Cookies["MessageModel"]) : null;
                if (hasCachedMessage && (Recipient == null || messageModel.DisplayName == Recipient))
                {
                    ViewBag.MessageModel = messageModel;
                    Response.Cookies.Delete("MessageModel");
                } else
                {
                    ViewBag.MessageModel = new PrivateMessageViewModel()
                    {
                        DisplayName = Recipient,
                        Summary = "",
                        Contents = "<p>&nbsp;</p>"
                    };
                }
            }
            else if (MailboxItemID != null) // If the mailbox item is specified in the URL, start the modal with that item open
            {
                MailboxItem mailboxItem = MailboxTypeID != (byte) EnumMailboxType.Moderation ? (MailboxItem) ((UserMailbox) mailboxes.SingleOrDefault(m => m.MailboxTypeID == (byte)MailboxTypeID)).UserItems
                    .SingleOrDefault(i => i.MailboxItemID == MailboxItemID) : _moderationMailbox.Find((int) MailboxItemID);
                if (mailboxItem != null)
                {
                    if (mailboxItem is UserMailboxItem) // If the item is a user mailbox item, initialize the reply
                        ViewBag.MessageModel = new PrivateMessageViewModel()
                        {
                            Item = mailboxItem,
                            DisplayName = mailboxItem.Message.SenderUser.DisplayName,
                            Summary = "Re: " + mailboxItem.Message.Summary.Substring(0, Math.Min(mailboxItem.Message.Summary.Length, 60)),
                            Contents = "<p><em>Quote from Admin on " + mailboxItem.Message.SentDate + ":</em><blockquote>" + mailboxItem.Message.Contents + "</blockquote><p>&nbsp;</p>"
                        };
                    else { // If the item is a moderation mailbox item
                        if (user.Role == "Moderator") // If the user is a moderator, set assignable users to self
                        {
                            List<ApplicationUser> assignableUsers = new List<ApplicationUser>() { user };
                            if ((mailboxItem as ModerationMailboxItem).AssigneeUserID != null && (mailboxItem as ModerationMailboxItem).AssigneeUserID != user.Id)
                                assignableUsers.Add((mailboxItem as ModerationMailboxItem).AssigneeUser);
                            ViewBag.AssignableUsers = assignableUsers;
                        } else // If the user is an administrator, set assignable users to all staff
                            ViewBag.AssignableUsers = _users.GetAll().Where(u => (u.Role == "Moderator" || u.Role == "Administrator")).OrderBy(u => u.DisplayName).OrderBy(u => u.Role);
                        ViewBag.MessageModel = new ReportActionViewModel() // Initialize report action model
                        {
                            Item = mailboxItem,
                            AssigneeDisplayName = (mailboxItem as ModerationMailboxItem).AssignedDate != null ? (mailboxItem as ModerationMailboxItem).AssigneeUser.DisplayName : user.DisplayName,
                            Contents = (mailboxItem as ModerationMailboxItem).ResolvedDate != null ? (mailboxItem as ModerationMailboxItem).ActionTaken : ""
                        };
                    }
                    ViewData["ActiveItem"] = MailboxItemID;
                }
            }
            else if (privateMessage != null) // If the private message is specified, include it in the ViewBag
                ViewBag.PrivateMessage = privateMessage;

            ViewBag.Mailboxes = mailboxes;

            return View();
        }

        // Private message sent
        // POST: /Mailbox/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PrivateMessageViewModel PrivateMessageViewModel, string returnUrl = null)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            ApplicationUser recipient = null;
            IEnumerable<Mailbox> mailboxes = _mailboxes.GetAllForUser(user); // Add user mailboxes
            if (user.Role == "Moderator" || user.Role == "Administrator") // Add moderation mailbox for moderators and administrators
            {
                mailboxes = mailboxes.Append(_moderationMailbox);
            }
            ViewBag.Mailboxes = mailboxes;

            // If the message was sent successfully and the recipient exists
            if (ModelState.IsValid && (recipient = await _users.FindByDisplayNameAsync((PrivateMessageViewModel as PrivateMessageViewModel).DisplayName)) != null)
            {
                // Create and send the new message
                PrivateMessage newMessage = new PrivateMessage()
                {
                    SenderUserID = user.Id,
                    RecipientUserID = recipient.Id,
                    Summary = (PrivateMessageViewModel as PrivateMessageViewModel).Summary.Trim(),
                    Contents = (PrivateMessageViewModel as PrivateMessageViewModel).Contents
                };
                newMessage.Send(_privateMessages, _mailboxes);
            } else // Add the failed message to the user's cookies to restore it and show error message(s)
                Response.Cookies.Append("MessageModel", JsonConvert.SerializeObject(PrivateMessageViewModel));

            // Reload the page with the mailbox open
            if (Url.IsLocalUrl(returnUrl))
            {
                returnUrl += (returnUrl.LastIndexOf("?") > -1 ? "&" : "?") + "modal=Mailbox";
                return Redirect(returnUrl);
            }
            else
                return Redirect("/?modal=Mailbox");
        }

        // New message loaded in mailbox modal
        // GET: /Mailbox/NewMessage/
        [HttpGet]
        public ActionResult NewMessage(int? OriginalPrivateMessageID, string returnUrl = null)
        {
            ViewData["OriginalPrivateMessageID"] = OriginalPrivateMessageID;
            ViewData["ReturnUrl"] = returnUrl;

            // Retrieve incomplete/failed message from cookies if any
            if (Request.Cookies.Any(c => c.Key == "MessageModel"))
            {
                PrivateMessageViewModel savedModel = JsonConvert.DeserializeObject<PrivateMessageViewModel>(Request.Cookies["MessageModel"]);
                Response.Cookies.Delete("MessageModel");
                return View(savedModel);
            }

            return View();
        }

        // Mailbox item loaded in mailbox modal
        // GET: /Mailbox/MailboxItem/MailboxTypeID/MailboxItemID
        [HttpGet]
        public async Task<IActionResult> MailboxItem(int MailboxTypeID, int MailboxItemID, string returnUrl = null)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            MailboxItem mailboxItem = MailboxTypeID != (int) EnumMailboxType.Moderation ? (MailboxItem) _mailboxes.Find(user.Id, (byte) MailboxTypeID).UserItems.SingleOrDefault(i => i.MailboxItemID == MailboxItemID)
                : _moderationMailbox.Find(MailboxItemID);
            Message message;
            // User and mailbox item are valid and the user has permission to view it
            if (user != null && mailboxItem != null && (message = mailboxItem.Message) != null && (mailboxItem is UserMailboxItem && (user.Id == message.SenderUserID 
                || user.Id == (message as PrivateMessage).RecipientUserID) || user.Role == "Moderator" || user.Role == "Administrator"))
            {
                if (!mailboxItem.ReadFlag) // Item is unread: mark it read
                {
                    mailboxItem.ReadFlag = true;
                    user.LastActivityDate = DateTime.Now;
                    if (mailboxItem is UserMailboxItem)
                        _mailboxes.Update((mailboxItem as UserMailboxItem).Mailbox);
                    else
                        _moderationMailbox.Update(mailboxItem as ModerationMailboxItem);
                }

                ViewData["ReturnUrl"] = returnUrl;
                // Add updated unread count to ViewData
                ViewData["UnreadCount"] = Math.Min(_mailboxes.Find(user.Id, (byte) EnumMailboxType.Inbox).Items.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count(), 99);
                if (user.Role == "Moderator" || user.Role == "Administrator") // Add updated moderation unread count to ViewData for staff
                    ViewData["ModerationUnreadCount"] = Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99);

                if (MailboxTypeID == (byte) EnumMailboxType.Moderation) // Mailbox item is a moderation item
                {
                    if (user.Role == "Moderator") // User is a moderator: set assignable users to self
                    {
                        List<ApplicationUser> assignableUsers = new List<ApplicationUser>() { user };
                        if ((mailboxItem as ModerationMailboxItem).AssigneeUserID != null && (mailboxItem as ModerationMailboxItem).AssigneeUserID != user.Id)
                            assignableUsers.Add((mailboxItem as ModerationMailboxItem).AssigneeUser);
                        ViewBag.AssignableUsers = assignableUsers;
                    }
                    else // User is an administrator: set assignable users to all staff
                        ViewBag.AssignableUsers = _users.GetAll().Where(u => (u.Role == "Moderator" || u.Role == "Administrator")).OrderBy(u => u.DisplayName).OrderBy(u => u.Role);
                }

                MessageViewModel messageViewModel = MailboxTypeID != (byte) EnumMailboxType.Moderation ? (MessageViewModel) new PrivateMessageViewModel()
                {
                    Item = mailboxItem,
                    DisplayName = message.SenderName,
                    Summary = "Re: " + message.Summary.Substring(0, Math.Min(message.Summary.Length, 60)),
                    Contents = "<p><em>Quote from " + message.SenderName + " on " + message.SentDate + ":</em><blockquote>" + message.Contents + "</blockquote><p>&nbsp;</p>"
                } : new ReportActionViewModel()
                {
                    Item = mailboxItem,
                    AssigneeDisplayName = (mailboxItem as ModerationMailboxItem).AssignedDate != null ? (mailboxItem as ModerationMailboxItem).AssigneeUser.DisplayName : user.DisplayName,
                    Contents = (mailboxItem as ModerationMailboxItem).ResolvedDate != null ? (mailboxItem as ModerationMailboxItem).ActionTaken : ""
                };

                return View((MailboxTypeID != (byte) EnumMailboxType.Moderation ? "User" : "Moderation") + "MailboxItem", messageViewModel);
            }
            return Redirect("Home/Error");
        }

        [HttpPost]
        public async Task<JsonResult> StarItem(byte MailboxTypeID, int MailboxItemID, bool IsUndo)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            bool success = false, isModerator = user.Role == "Moderator" || user.Role == "Administrator";
            string errorMessage = "";
            UserMailboxItem item = _mailboxes.Find(user.Id, MailboxTypeID).UserItems.SingleOrDefault(i => i.MailboxItemID == MailboxItemID);
            int starredItemID = 0;
            
            if ((!IsUndo && item.StarredDate == null) || (IsUndo && item.StarredDate != null))
            {
                user.LastActivityDate = DateTime.Now;
                success = true;

                if (!IsUndo)
                    starredItemID = item.Star(_mailboxes);
                else
                    item.Unstar(_mailboxes);
            } else
                errorMessage = "You may not " + (IsUndo ? "un" : "") + "star a" + (IsUndo ? "n un" : " ") + "starred item!";

            return Json(new object[] { success, success ? _mailboxes.Find(user.Id, 0).UserItems.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count().ToString() : errorMessage,
                success && isModerator ? Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99).ToString() : null, starredItemID });
        }

        [HttpPost]
        public async Task<JsonResult> MarkItemsAsRead(byte MailboxTypeID, string MailboxItemIDs, bool IsUndo)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            bool success = false, isModerator = user.Role == "Moderator" || user.Role == "Administrator", isUserMailbox;
            string errorMessage = "";
            Mailbox mailbox = (isUserMailbox = MailboxTypeID != (byte) EnumMailboxType.Moderation) ? (Mailbox) _mailboxes.Find(user.Id, MailboxTypeID) : _moderationMailbox;
            int[] itemIDs = JsonConvert.DeserializeObject<int[]>(MailboxItemIDs);

            // Allow marking if the items being marked are from a user tab or if the items are moderation items and the user is an administrator
            if (isUserMailbox || user.Role == "Administrator")
            {
                user.LastActivityDate = DateTime.Now;
                success = true;

                foreach (int itemID in itemIDs)
                {
                    MailboxItem item = mailbox.Items.SingleOrDefault(i => i.MailboxItemID == itemID);
                    if (item != null && (!isUserMailbox || (item as UserMailboxItem).UserID == user.Id))
                    {
                        if (isUserMailbox)
                            (item as UserMailboxItem).MarkAsRead(_mailboxes, IsUndo);
                        else
                            (item as ModerationMailboxItem).MarkAsRead(_moderationMailbox, IsUndo);
                    }
                }
            } else
                errorMessage = "You do not have the rights to mark moderation mailbox items as " + (IsUndo ? "un" : "") + "read!";

            return Json(new object[] { success, success ? _mailboxes.Find(user.Id, 0).UserItems.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count().ToString() : errorMessage,
                success && isModerator ? Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99).ToString() : null });
        }

        [HttpPost]
        public async Task<JsonResult> MarkItemsAsSpam(byte MailboxTypeID, string MailboxItemIDs, bool IsUndo)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            bool success = false, isModerator = user.Role == "Moderator" || user.Role == "Administrator";
            string errorMessage = "";
            UserMailbox mailbox = _mailboxes.Find(user.Id, MailboxTypeID);
            int[] itemIDs = JsonConvert.DeserializeObject<int[]>(MailboxItemIDs);
            List<int> spamItemIDs = new List<int>();

            // Allow marking if the items are being marked as spam and belong to the inbox tab or if the items are being removed from spam and belong to the spam tab
            if ((!IsUndo && MailboxTypeID == (byte) EnumMailboxType.Inbox) || (IsUndo && MailboxTypeID == (byte) EnumMailboxType.Spam))
            {
                user.LastActivityDate = DateTime.Now;
                success = true;

                foreach (int itemID in itemIDs)
                {
                    UserMailboxItem item = mailbox.UserItems.SingleOrDefault(i => i.MailboxItemID == itemID);
                    int spamItemID = 0;
                    if (item != null && item.UserID == user.Id)
                    {
                        if (!IsUndo)
                            spamItemID = item.MarkAsSpam(_mailboxes);
                        else
                            item.RemoveFromSpam(_mailboxes);
                    }
                    spamItemIDs.Add(spamItemID);
                }
            }
            else
                errorMessage = "You may only " + (IsUndo ? "remove spam inbox items from spam!" : "mark inbox items as spam!");

            return Json(new object[] { success, success ? _mailboxes.Find(user.Id, 0).UserItems.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count().ToString() : errorMessage,
                success && isModerator ? Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99).ToString() : null, IsUndo ? null : spamItemIDs.ToArray() });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteItems(byte MailboxTypeID, string MailboxItemIDs, bool isUndo)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            bool success = false, isModerator = user.Role == "Moderator" || user.Role == "Administrator", isUserMailbox;
            string errorMessage = "";
            Mailbox mailbox = (isUserMailbox = MailboxTypeID != (byte)EnumMailboxType.Moderation) ? (Mailbox)_mailboxes.Find(user.Id, MailboxTypeID) : _moderationMailbox;
            int[] itemIDs = JsonConvert.DeserializeObject<int[]>(MailboxItemIDs);
            List<int> deletedItemIDs = new List<int>();

            // Allow deletion of only user mailbox items or moderation items is the user is an Administrator
            if (isUserMailbox || user.Role == "Administrator")
            {
                // If items are user mailbox items, only allow the action if deleting inbox items, deleting sent items, deleting spam items, or undeleting deleted items
                if (!isUserMailbox || ((!isUndo && (MailboxTypeID == (byte)EnumMailboxType.Inbox || MailboxTypeID == (byte)EnumMailboxType.Sent || MailboxTypeID == (byte)EnumMailboxType.Spam))
                    || (isUndo && MailboxTypeID == (byte)EnumMailboxType.Deleted))) {
                    user.LastActivityDate = DateTime.Now;
                    success = true;

                    foreach (int itemID in itemIDs)
                    {
                        MailboxItem item = mailbox.Items.SingleOrDefault(i => i.MailboxItemID == itemID);
                        int deletedItemID = 0;
                        if (item != null && (!isUserMailbox || (item as UserMailboxItem).UserID == user.Id))
                        {
                            if (isUserMailbox)
                            {
                                if (!isUndo)
                                    deletedItemID = (item as UserMailboxItem).Delete(_mailboxes);
                                else
                                    (item as UserMailboxItem).Undelete(_mailboxes);
                            }
                            else if (!isUndo)
                                (item as ModerationMailboxItem).Delete(_moderationMailbox);
                        }
                        deletedItemIDs.Add(deletedItemID);
                    }
                } else
                    errorMessage = "You may only " + (isUndo ? "undelete deleted" : "delete inbox, sent, or spam") + " items!";
            }
            else
                errorMessage = "You do not have the rights to delete moderation mailbox items!";

            return Json(new object[] { success, success ? _mailboxes.Find(user.Id, 0).UserItems.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count().ToString() : errorMessage,
                success && isModerator ? Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99).ToString() : null, isUndo ? null : deletedItemIDs.ToArray() });
        }

        [HttpPost]
        public async Task<JsonResult> SearchItems(byte MailboxTypeID, string SearchTerm)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            bool success = false, isModerator = user.Role == "Moderator" || user.Role == "Administrator", isUserMailbox;
            string errorMessage = "";
            Mailbox mailbox = (isUserMailbox = MailboxTypeID != (byte) EnumMailboxType.Moderation) ? (Mailbox)_mailboxes.Find(user.Id, MailboxTypeID) : _moderationMailbox;
            int[] mailboxItemIDs = null;

            // Allow marking if the items being marked are from a user tab or if the items are moderation items and the user is an administrator
            if (isUserMailbox || user.Role == "Administrator")
            {
                success = true;

                IEnumerable<SearchResult<MailboxItem>> searchResults = SearchResult<MailboxItem>.GetSearchResults(mailbox.Items, SearchTerm);
                mailboxItemIDs = searchResults.Select(r => r.Object.MailboxItemID).ToArray();
            }
            else
                errorMessage = "You do not have the rights to search moderation mailbox items!";

            return Json(new object[] { success, success ? null : errorMessage, mailboxItemIDs.ToArray() });
        }

        [HttpGet]
        public async Task<IActionResult> PrivateMessage(long PrivateMessageID)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            PrivateMessage privateMessage = _privateMessages.Find(PrivateMessageID);
            if (user != null && privateMessage != null && (user.Role == "Moderator" || user.Role == "Administrator"))
            {
                return View(privateMessage);
            }
            return Redirect("Home/Error");
        }

        [HttpGet]
        public async Task<IActionResult> ModerationMailboxItem(int MailboxItemID)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            ModerationMailboxItem mailboxItem = _moderationMailbox.GetAll().SingleOrDefault(i => i.MailboxItemID == MailboxItemID);
            if (user != null && mailboxItem != null && (user.Role == "Moderator" || user.Role == "Administrator"))
            {
                return View(mailboxItem);
            }
            return Redirect("Home/Error");
        }

        [HttpPost]
        public async Task<JsonResult> AssignItemToUser(int MailboxItemID, string DisplayName, bool IsUndo)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            ApplicationUser assignee = await _users.FindByDisplayNameAsync(DisplayName);
            bool success = false, isModerator = user.Role == "Moderator" || user.Role == "Administrator";
            string errorMessage = "";
            ModerationMailboxItem item = (ModerationMailboxItem) _moderationMailbox.Items.SingleOrDefault(i => i.MailboxItemID == MailboxItemID);

            if (isModerator && (user.Id == assignee.Id || user.Role == "Administrator"))
            {
                if ((!IsUndo && item.AssignedDate == null) || (IsUndo && item.AssignedDate != null))
                {
                    bool isUnresolved = true;
                    if (!IsUndo || (isUnresolved = item.ResolvedDate == null) || user.Role == "Administrator")
                    {
                        user.LastActivityDate = DateTime.Now;
                        success = true;

                        if (!IsUndo)
                            item.AssignToUser(_moderationMailbox, assignee, user);
                        else
                        {
                            if (!isUnresolved)
                                item.MarkAsUnresolved(_moderationMailbox);
                            item.Unassign(_moderationMailbox);
                        }
                    }
                    else
                        errorMessage = "You do not have the rights to unassign a resolved item!";
                }
                else
                    errorMessage = "You may not " + (IsUndo ? "un" : "") + "assign an " + (IsUndo ? "un" : "") + "assigned item!";
            } else
                errorMessage = "You do not have the rights to " + (IsUndo ? "un" : "") + "assign '" + DisplayName + "' " + (IsUndo ? "from" : "to") + " this item!";

            return Json(new object[] { success, success ? _mailboxes.Find(user.Id, 0).UserItems.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count().ToString() :
                errorMessage, success ? Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99).ToString() : null, success != IsUndo ? assignee != null ?
                item.AssigneeUser.GetFullFormattedDisplayName(null, true, "?Modal=Mailbox/" + (byte) EnumMailboxType.Moderation + "/" + MailboxItemID) :
                DisplayName : null, item.AssignedDate != null ? ((DateTime) item.AssignedDate).ToLocalTime().ToString() : null });
        }

        [HttpPost]
        public async Task<JsonResult> ResolveItem(int MailboxItemID, string ActionTaken, bool IsUndo)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            bool success = false;
            string errorMessage = "";
            ModerationMailboxItem item = (ModerationMailboxItem)_moderationMailbox.Items.SingleOrDefault(i => i.MailboxItemID == MailboxItemID);

            if (item.AssigneeUserID != null)
            {
                if (user.Id == item.AssigneeUserID || (IsUndo && user.Role == "Administrator"))
                {
                    if ((!IsUndo && item.ResolvedDate == null) || (IsUndo && item.ResolvedDate != null))
                    {
                        if (IsUndo || ActionTaken != null)
                        {
                            user.LastActivityDate = DateTime.Now;
                            success = true;

                            if (!IsUndo)
                                item.MarkAsResolved(_moderationMailbox, ActionTaken);
                            else
                                item.MarkAsUnresolved(_moderationMailbox);
                        }
                        else
                            errorMessage = "You must enter the action you took towards the report in order to resolve it!";
                    }
                    else
                        errorMessage = "You may not mark a" + (IsUndo ? "n un" : " ") + "resolved item as " + (IsUndo ? "un" : "") + "resolved!";
                }
                else
                    errorMessage = "You must be assigned to this item to mark it as " + (IsUndo ? "un" : "") + "resolved!";
            }
            else
                errorMessage = "You may not mark an unassigned item as " + (IsUndo ? "un" : "") + "resolved!";

            return Json(new object[] { success, success ? _mailboxes.Find(user.Id, 0).UserItems.Where(i => (i as UserMailboxItem).MarkedAsSpamDate == null && i.DeletedDate == null && !i.ReadFlag).Count().ToString() :
                errorMessage, success ? Math.Min(_moderationMailbox.Items.Where(i => i.DeletedDate == null && !i.ReadFlag).Count(), 99).ToString() : null, item.AssigneeUserID != null ?
                item.AssigneeUser.GetFullFormattedDisplayName(null, true, "?Modal=Mailbox/" + (byte) EnumMailboxType.Moderation + "/" + MailboxItemID) : null,
                item.ResolvedDate != null ? ((DateTime) item.ResolvedDate).ToLocalTime().ToString() : null });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ReportItem(byte ReportTypeID, string ReportedItemID, string returnUrl = null)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            IReportable reportedItem;
            string[] keys;

            ViewData["ReturnUrl"] = returnUrl;

            switch ((EnumReportType) ReportTypeID)
            {
                case EnumReportType.Question:
                    reportedItem = _questions.Find(int.Parse(ReportedItemID));
                    break;
                case EnumReportType.Answer:
                    keys = ReportedItemID.Split('-');
                    reportedItem = _questions.Find(int.Parse(keys[0])).Posts.SingleOrDefault(p => p.QuestionPostID == int.Parse(keys[1]));
                    break;
                case EnumReportType.Comment:
                    keys = ReportedItemID.Split('-');
                    reportedItem = _questions.Find(int.Parse(keys[0])).Posts.SingleOrDefault(p => p.QuestionPostID == int.Parse(keys[1])).Comments.SingleOrDefault(c => c.PostCommentID == short.Parse(keys[2]));
                    break;
                case EnumReportType.User_Profile:
                    reportedItem = await _users.FindByDisplayNameAsync(ReportedItemID);
                    ReportedItemID = (reportedItem as ApplicationUser).Id;
                    break;
                default:
                    reportedItem = _privateMessages.Find(long.Parse(ReportedItemID));
                    break;
            }

            if (reportedItem != null)
            {
                return View(new ReportViewModel()
                {
                    ReportedItem = reportedItem,
                    ReporterDisplayName = user != null ? user.DisplayName : null,
                    ReportTypeID = ReportTypeID,
                    ReportedItemID = ReportedItemID,
                    ReportReason = "",
                    AcceptRules = false
                });
            }

            return Redirect("Home/Error");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReportItem(ReportViewModel ReportViewModel, string returnUrl = null)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            IReportable reportedItem;
            string[] keys = ReportViewModel.ReportedItemID.Split('-');

            if (ModelState.IsValid)
            {
                Report report = new Report()
                {
                    ReportTypeID = ReportViewModel.ReportTypeID,
                    ReportReason = ReportViewModel.ReportReason,
                    SenderUserID = user != null ? user.Id : null
                };

                switch ((EnumReportType) ReportViewModel.ReportTypeID)
                {
                    case EnumReportType.Question:
                        reportedItem = _questions.Find(int.Parse(keys[0]));
                        if (reportedItem != null)
                            report.ReportedQuestionID = (reportedItem as Question).QuestionID;
                        break;
                    case EnumReportType.Answer:
                        reportedItem = _questions.Find(int.Parse(keys[0])).Posts.SingleOrDefault(p => p.QuestionPostID == int.Parse(keys[1]));
                        if (reportedItem != null)
                        {
                            report.ReportedQuestionID = (reportedItem as QAPost).QuestionID;
                            report.ReportedQuestionPostID = (reportedItem as QAPost).QuestionPostID;
                        }
                        break;
                    case EnumReportType.Comment:
                        reportedItem = _questions.Find(int.Parse(keys[0])).Posts.SingleOrDefault(p => p.QuestionPostID == int.Parse(keys[1])).Comments.SingleOrDefault(c => c.PostCommentID == short.Parse(keys[2]));
                        if (reportedItem != null)
                        {
                            report.ReportedQuestionID = (reportedItem as Comment).QuestionID;
                            report.ReportedQuestionPostID = (reportedItem as Comment).QuestionPostID;
                            report.ReportedPostCommentID = (reportedItem as Comment).PostCommentID;
                        }
                        break;
                    case EnumReportType.User_Profile:
                        reportedItem = await _users.FindByDisplayNameAsync(keys[0]);
                        if (reportedItem != null)
                            report.ReportedUserID = (reportedItem as ApplicationUser).Id;
                        break;
                    default:
                        reportedItem = _privateMessages.Find(long.Parse(keys[0]));
                        if (reportedItem != null)
                            report.ReportedPrivateMessageID = (reportedItem as PrivateMessage).PrivateMessageID;
                        break;
                }

                if (reportedItem != null)
                    report.Send(_reports, _moderationMailbox);
            }

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return Redirect("/");
        }
    }
}
