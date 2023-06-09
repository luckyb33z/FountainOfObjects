using System;

namespace IDescriptive
{
    public interface IDescriptive
    {
        string InRoomDescription { get; set; } 
        ConsoleColor DescColor { get; set; }
    }

    public interface IDescriptiveNoisy: IDescriptive
    {
        string AdjacentDescription { get; set; }
        int PerceptibleDistance { get; set; }
    }
}