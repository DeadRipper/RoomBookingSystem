namespace RBA.Models.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName
        {
            get => field;
            set => field = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        } = string.Empty;
    }
}