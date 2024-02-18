namespace WebApplication22.Models
{
    public class ProjectTask
    {
       public int  ProjectTaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }

        //Navigation Property
        
        // This property allows for easy access to the related Project entity from a Task entity
        public Project? Project { get; set; }

    }
}
