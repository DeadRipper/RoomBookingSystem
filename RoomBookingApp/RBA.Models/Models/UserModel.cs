namespace RBA.Models.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName
        {
            get;
            set
            {
                if (value == null || string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
            }
        }
    }
}