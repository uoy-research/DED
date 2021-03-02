
using Improbable;

namespace Vehicles
{
    public static class Acls
    {
        static readonly WorkerAttributeSet DefaultWorker = new WorkerAttributeSet(new Improbable.Collections.List<string> { "default_worker" });
        static readonly WorkerAttributeSet PlayerAISet = new WorkerAttributeSet(new Improbable.Collections.List<string> { "player_ai" });

        public static readonly WorkerRequirementSet PlayerAI = new WorkerRequirementSet(new Improbable.Collections.List<WorkerAttributeSet> { PlayerAISet });
    }
}
