using coretest.Domain.Models;

namespace coretest.Domain.Services.Communication
{
    public class CreateUserResponse : BaseResponse
    {
        public User User { get; private set; }

        private CreateUserResponse(bool success, string message, User user) : base(success, message)
        {
            User = user;
        }

        // Creates a success response.
        // param username="user" Saved user
        public CreateUserResponse(User user) : this(true, string.Empty, user)
        { }

        // Creates am error response.
        // param name="message" Error message
        public CreateUserResponse(string message) : this(false, message, null)
        { }
    }
}
