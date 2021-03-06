using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class piercingGrenade : BaseProjectile
	{
		[Ordinal(0)]  [RED("alive")] public CBool Alive { get; set; }
		[Ordinal(1)]  [RED("energyLossFactor")] public CFloat EnergyLossFactor { get; set; }
		[Ordinal(2)]  [RED("explosionEffect")] public CHandle<gameEffectInstance> ExplosionEffect { get; set; }
		[Ordinal(3)]  [RED("gravitySimulation")] public CFloat GravitySimulation { get; set; }
		[Ordinal(4)]  [RED("grenadeLifetime")] public CFloat GrenadeLifetime { get; set; }
		[Ordinal(5)]  [RED("pierceTime")] public CFloat PierceTime { get; set; }
		[Ordinal(6)]  [RED("piercingEffect")] public gameEffectRef PiercingEffect { get; set; }
		[Ordinal(7)]  [RED("startVelocity")] public CFloat StartVelocity { get; set; }
		[Ordinal(8)]  [RED("trailEffectName")] public CName TrailEffectName { get; set; }

		public piercingGrenade(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
