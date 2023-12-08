using AirlineSeatReservationSystem.Entity;

namespace  AirlineSeatReservationSystem.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users {get;}

        void CreateUser(User user);

    }
}