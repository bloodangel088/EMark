using System;

namespace EMark.DataAccess.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid JwtId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsInvalidated { get; set; }
        public int UserId { get; set; }
    }
}