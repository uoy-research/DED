//entity_manager.h

#ifndef ENTITY_MANAGER_H
#define ENTITY_MANAGER_H

namespace operations {
	namespace entities {

		class EntityManager : public Entity {

		public:

			template <typename T>
			void Add( const typename T::Data& data ) {
				Entity::Add<T>( data );
			}

			template <typename T>
			void Add( typename T::Data&& data ) {
				Entity::Add<T>( data );
			}

			template <typename T>
			void UpdateComponent( const ComponentUpdateOp<T>& op ) {
				Update<T>( op.Update );
			}

			template <typename T>
			void UpdateComponent( const typename T::Update& update ) {
				Update<T>( update );
			}

			template<typename T>
			Option<typename T::Data&> GetComponentData( ) {
				return Get<T>( );
			}
		};
	}
}


#endif // !ENTITY_MANAGER_H