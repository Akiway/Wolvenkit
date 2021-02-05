using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class AIArgumentSerializableValue : AIArgumentDefinition
	{
		[Ordinal(0)]  [RED("type")] public CEnum<AIArgumentType> Type { get; set; }
		[Ordinal(1)]  [RED("defaultValue")] public CHandle<ISerializable> DefaultValue { get; set; }

		public AIArgumentSerializableValue(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
