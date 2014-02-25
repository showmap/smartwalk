namespace SmartWalk.Shared.DataContracts
{
    public interface IReference
    {
        int Id { get; set; }

        string Storage { get; set; }

        /// <summary>
        /// Gets or sets the type of Reference if the entity that's being referenced is a Host or a Venue. 
        /// For external events the type is always 0.
        /// </summary>
        /// <value>The type.</value>
        int? Type { get; set; }
    }
}