using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coretest.Domain.Models;

namespace coretest.Domain.Services.Communication
{
    public class CreateUserResponse : BaseResponse
    {
        public User User { get; private set; }
        public Auth Auth { get; private set; }

        private CreateUserResponse(bool success, string message, User user, Auth auth) : base(success, message)
        {
            User = user;
            Auth = auth;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param username="user">Saved user.</param>
        /// <returns>Response.</returns>
        public CreateUserResponse(User user) : this(true, string.Empty, user, null)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CreateUserResponse(string message) : this(false, message, null, null)
        { }

        public CreateUserResponse(Auth auth) : this(true, string.Empty, null, auth)
        { }
    }
}
