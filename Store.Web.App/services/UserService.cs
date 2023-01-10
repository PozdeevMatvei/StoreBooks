﻿using Microsoft.AspNetCore.Http;
using Store.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.App.services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepository _userRepository;
        public UserService(IHttpContextAccessor contextAccessor, IUserRepository userRepository)
        {
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
        }

        public async Task<(bool isGetUser, User? user)> TryGetUserAsync()
        {
            var (isAuthorization, userName) = TryGetAuthorizationUserName();

            if (!isAuthorization)
                return (false, null);

            var user = await _userRepository.GetUserAsync(userName!);
            return (true, user);
        }
        public async Task AddOrderAsync(OrderDto order)
        {
            var (isAuthorization, userName) = TryGetAuthorizationUserName();

            if (!isAuthorization)
                return;

            User user = await _userRepository.GetUserAsync(userName!);
            user.Orders.Add(order); 
            await _userRepository.UpdateAsync();
        }
        public async Task<bool> IsAuthorization()
        {
            var (isAuthorization, _) = await Task.Run(TryGetAuthorizationUserName);

            return isAuthorization;
        }
        public (bool isAuthorization, string? userName) TryGetAuthorizationUserName()
        {
            var claimPrincipal = _contextAccessor.HttpContext.User;
            string? userName = claimPrincipal.Identity?.Name;

            if(userName != null)          
                return (true, userName);

            return (false, null);
        }
    }
}
