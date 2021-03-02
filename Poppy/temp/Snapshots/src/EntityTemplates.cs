using Ded;
using Ded.Player;
using Dpge;
using Improbable;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Worker;
using Rde;

namespace Vehicles
{
    public struct RouteNode
    {
        public EntityId entityId;
        public Coordinates coordinates;
    }

    public static class EntityTemplates
    {
        public static Entity CreatePlayerEntity()
        {
            var writeAcl = new Map<uint, WorkerRequirementSet>
                { { Position.ComponentId, Acls.PlayerAI },
                    { PlotMap.ComponentId, Acls.PlayerAI },
                    { PlayerInfo.ComponentId, Acls.PlayerAI },
                    { ActorInfo.ComponentId, Acls.PlayerAI },
                    { Knowledge.ComponentId, Acls.PlayerAI },
                    { EntitySpawning.ComponentId, Acls.PlayerAI },
                    { Inventory.ComponentId, Acls.PlayerAI }
                };

            var entity = CreateBaseEntity(new Coordinates(0, 0, 0), "player", Acls.PlayerAI, writeAcl);
            entity.Add(new CurrentState.Data(Ded.State.CREATE_INFO_MAP, Ded.State.FIRST));
            entity.Add(new PlotMap.Data(new Map<EntityId, Plot>()));
            entity.Add(new PlayerInfo.Data());
            entity.Add(new ActorInfo.Data(new Map<Info, uint>(), new EntityId()));
            entity.Add(new Knowledge.Data(new Map<EntityId, KnowledgeItem>()));
            entity.Add(new EntitySpawning.Data(new Map<uint, EntityType>(), new Map<EntityId, EntityType>()));
            entity.Add(new Inventory.Data(0));

            return entity;
        }

        static Entity CreateBaseEntity(Coordinates coord, string name, WorkerRequirementSet readAcl, Map<uint, WorkerRequirementSet> writeAcl)
        {
            var entity = new Entity();
            entity.Add(new Position.Data(coord));
            entity.Add(new Persistence.Data());
            entity.Add(new Metadata.Data(name));

            AddAcl(entity, readAcl, writeAcl);

            return entity;
        }

        static void AddAcl(Entity entity, WorkerRequirementSet readAcl, Map<uint, WorkerRequirementSet> writeAcl)
        {
            var aclData = new EntityAclData
            {
                readAcl = readAcl,
                componentWriteAcl = writeAcl
            };
            entity.Add(new EntityAcl.Data(aclData));
        }

    }
}