namespace SmartWalk.Server.Models.XmlModel
{
    interface IEntity
    {
        string Name { get; set; }

        string Description { get; set; }

        string Logo { get; set; }

        Address[] Addresses { get; set; }

        Contact[] Contacts { get; set; }
    }
}