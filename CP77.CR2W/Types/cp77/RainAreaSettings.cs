using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class RainAreaSettings : IAreaSettings
	{
		[Ordinal(0)]  [RED("numParticles")] public CUInt32 NumParticles { get; set; }
		[Ordinal(1)]  [RED("radius")] public CFloat Radius { get; set; }
		[Ordinal(2)]  [RED("heightRange")] public CFloat HeightRange { get; set; }
		[Ordinal(3)]  [RED("globalLightResponse")] public CFloat GlobalLightResponse { get; set; }
		[Ordinal(4)]  [RED("tiling")] public curveData<CFloat> Tiling { get; set; }
		[Ordinal(5)]  [RED("speed")] public curveData<CFloat> Speed { get; set; }
		[Ordinal(6)]  [RED("roughnessShift")] public CFloat RoughnessShift { get; set; }
		[Ordinal(7)]  [RED("roughnessClip")] public CFloat RoughnessClip { get; set; }
		[Ordinal(8)]  [RED("roughnessExponent")] public CFloat RoughnessExponent { get; set; }
		[Ordinal(9)]  [RED("baseColorShift")] public CFloat BaseColorShift { get; set; }
		[Ordinal(10)]  [RED("baseColorClip")] public CFloat BaseColorClip { get; set; }
		[Ordinal(11)]  [RED("baseColorExponent")] public CFloat BaseColorExponent { get; set; }
		[Ordinal(12)]  [RED("porosityThreshold")] public CFloat PorosityThreshold { get; set; }
		[Ordinal(13)]  [RED("moistureAccumulationSpeed")] public CFloat MoistureAccumulationSpeed { get; set; }
		[Ordinal(14)]  [RED("puddlesAccumulationSpeed")] public CFloat PuddlesAccumulationSpeed { get; set; }
		[Ordinal(15)]  [RED("moistureEvaporationSpeed")] public CFloat MoistureEvaporationSpeed { get; set; }
		[Ordinal(16)]  [RED("puddlesEvaporationSpeed")] public CFloat PuddlesEvaporationSpeed { get; set; }
		[Ordinal(17)]  [RED("rainIntensity")] public curveData<CFloat> RainIntensity { get; set; }
		[Ordinal(18)]  [RED("rainOverride")] public curveData<CFloat> RainOverride { get; set; }
		[Ordinal(19)]  [RED("rainMoistureOverride")] public curveData<CFloat> RainMoistureOverride { get; set; }
		[Ordinal(20)]  [RED("rainPuddlesOverride")] public curveData<CFloat> RainPuddlesOverride { get; set; }
		[Ordinal(21)]  [RED("rainleaksMask")] public rRef<CBitmapTexture> RainleaksMask { get; set; }
		[Ordinal(22)]  [RED("raindropsMask")] public rRef<CBitmapTexture> RaindropsMask { get; set; }
		[Ordinal(23)]  [RED("rainRipplesMask")] public rRef<CBitmapTexture> RainRipplesMask { get; set; }

		public RainAreaSettings(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
