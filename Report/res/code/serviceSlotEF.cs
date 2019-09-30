public class ServiceSlot
{
    [Key] |\label{line:pkAn}|
    [Column(Order=1)]
    [DatabaseGenerated] |\label{line:valueGenAn}|
    public int Id { get; set; }
    public decimal? BudgetTarget { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

    [Key] |\label{line:pk2An}|
    [Column(Order=2)]
    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
    public Event Event { get; set; }

    [ForeignKey(nameof(Type))] |\label{line:fkAn}|
    public int TypeId { get; set; }
    public ServiceType Type { get; set; }
}
