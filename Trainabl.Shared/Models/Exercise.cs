namespace Trainabl.Shared.Models;

public class Exercise
{
	public int Index { get; set; }
	public string MovementName { get; set; }
	public int? Sets { get; set; }
	public int? Reps { get; set; }
	public string? Time { get; set; }
	
	public SetMeasureType SetMeasureType { get; set; }
	
	public Movement? Movement { get; set; }

	public override string ToString()
	{
		return SetMeasureType switch
		{
			SetMeasureType.Warmup  => $"{MovementName} warmup",
			SetMeasureType.Reps    => $"{MovementName}: {Sets} x {Reps}",
			SetMeasureType.Time    => $"{MovementName}: {Sets} for {Time}",
			SetMeasureType.Failure => $"{MovementName}: {Sets} to failure",
			_                      => throw new ArgumentOutOfRangeException()
		};
	}
}