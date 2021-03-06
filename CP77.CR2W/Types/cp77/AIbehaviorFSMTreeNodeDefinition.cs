using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class AIbehaviorFSMTreeNodeDefinition : AIbehaviorTreeNodeDefinition
	{
		[Ordinal(0)]  [RED("initialState")] public CHandle<AIbehaviorFSMStateDefinition> InitialState { get; set; }
		[Ordinal(1)]  [RED("states")] public CArray<CHandle<AIbehaviorFSMStateDefinition>> States { get; set; }
		[Ordinal(2)]  [RED("transitions")] public CArray<CHandle<AIbehaviorFSMTransitionDefinition>> Transitions { get; set; }

		public AIbehaviorFSMTreeNodeDefinition(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
