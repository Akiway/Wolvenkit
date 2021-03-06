using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class ReprimandEscalationEvent : redEvent
	{
		[Ordinal(0)]  [RED("startDeescalate")] public CBool StartDeescalate { get; set; }
		[Ordinal(1)]  [RED("startReprimand")] public CBool StartReprimand { get; set; }

		public ReprimandEscalationEvent(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
