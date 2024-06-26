﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Data;
using Rhythmify.Models;

namespace Rhythmify.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;

            return View();
        }

        public async Task<ActionResult> Show(string id)
        {
            User user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            return View(user);
        }

        public async Task<ActionResult> Edit(string id)
        {
            User user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); // Lista de nume de roluri

            // Cautam ID-ul rolului in baza de date
            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); // Selectam 1 singur rol
            ViewBag.UserRole = currentUserRole;

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, User newData, [FromForm] string newRole)
        {
            User user = db.Users.Find(id);

            System.Diagnostics.Debug.WriteLine(id);
            System.Diagnostics.Debug.WriteLine(newRole);
            System.Diagnostics.Debug.WriteLine(newData.DisplayName);

            user.AllRoles = GetAllRoles();


            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.DisplayName = newData.DisplayName;

                // Cautam toate rolurile din baza de date
                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    // Scoatem userul din rolurile anterioare
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                // Adaugam noul rol selectat
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Invalid");
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users.Include("Posts")
                 .Include("Playlists")
                 .Include("Conversations")
                 .Include("Connections")
                 .Where(u => u.Id == id)
                 .FirstOrDefault();

            if (user == null)
            {
                // Handle user not found, possibly return an error view or message
                return NotFound();
            }

            // Delete user posts
            if (user.Posts.Count > 0)
            {
                foreach (var post in user.Posts)
                {
                    db.Posts.Remove(post);
                }
            }

            // Delete user playlists
            if (user.Playlists.Count > 0)
            {
                foreach (var playlist in user.Playlists)
                {
                    db.Playlists.Remove(playlist);
                }
            }

            // Delete user conversations
            var conversations = db.Conversations
                                  .Where(c => c.User1Id == id || c.User2Id == id)
                                  .ToList();
            if (conversations.Count > 0)
            {
                foreach (var conversation in conversations)
                {
                    db.Conversations.Remove(conversation);
                }
            }

            // Delete user connections
            var connections = db.Connections
                                .Where(c => c.UserId == id || c.FriendId == id)
                                .ToList();
            if (connections.Count > 0)
            {
                foreach (var connection in connections)
                {
                    db.Connections.Remove(connection);
                }
            }

            // Remove the user
            db.Users.Remove(user);

            // Save changes to the database
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}