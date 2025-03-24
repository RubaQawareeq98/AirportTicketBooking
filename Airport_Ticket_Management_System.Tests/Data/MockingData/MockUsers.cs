using Model.Users;

namespace Airport_Ticket_Management_System.Tests.Data.MockingData;

public abstract class MockUsers
{
    public static List<User> GetMockUsers()
    {
       return
       [
           new User
           {
               Id = new Guid("3da9efb3-185c-4233-bf4e-bbc0ece85484"),
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