namespace Trainabl.Shared.Models;

public class Exercise
{
	public Movement Movement { get; set; }
	public string MovementName { get; set; }
	public int? Sets { get; set; }
	public string? SetMeasure { get; set; }
	
	public SetMeasureType SetMeasureType { get; set; }

	public override string ToString()
	{
		return SetMeasureType switch
		{
			SetMeasureType.Warmup  => $"{MovementName} warmup",
			SetMeasureType.Reps    => $"{MovementName}: {Sets} x {SetMeasure}",
			SetMeasureType.Time    => $"{MovementName}: {Sets} for {SetMeasure}",
			SetMeasureType.Failure => $"{MovementName}: {Sets} to failure",
			_                      => throw new ArgumentOutOfRangeException()
		};
	}
}