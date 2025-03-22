using Model.Users;

namespace Airport_Ticket_Management_System.Tests.Data.MockingData;

public class MockUsers
{
    public static List<User> GetMockUsers()
    {
       return
       [
           new User
           {
               Id = Guid.NewGuid(),
               UserName = "ruba",
               Password = "123456",
               FullName = "Ruba",
               Role = UserRole.Passenger
           },

           new User
           {
               Id = Guid.NewGuid(),
               UserName = "ali",
               Password = "123456",
               FullName = "Ali",
               Role = UserRole.Passenger
           }


       ];
    }
}