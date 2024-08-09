namespace Admin_Dashboard.Models
{
    public class UserRoleViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
    }
}
