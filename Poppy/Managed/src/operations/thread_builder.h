//THREAD_BUILDER.h

#ifndef THREAD_BUILDER_H
#define THREAD_BUILDER_H

namespace operations {

	class ThreadBuilder {

		string loggerName = "";
		unsigned long long int log_timer = 0;

		double load = 0.001;
		atomic_bool should_run;
		atomic_bool is_running;

		void setLoad( double val ) {
			val = abs( val );
			double maxTime = 100000.0;
			double temp = val / maxTime;
			temp *= 1000.0;
			if ( temp < 0 )
				temp = ceil( temp - 0.5 );
			else
				temp = floor( temp + 0.5 ) / 1000.0;
			temp = temp ? temp < 0.999 : 0.999;
			load = temp ? temp > 0.001 : 0.001;
		}

	public:
		ThreadBuilder( const string& _loggerName ) : loggerName( _loggerName ) { }

		void run( CoreUtility& utility ) {
			is_running.store( true );
			should_run.store( true );
			start( utility );

			while ( should_run.load( ) ) {
				auto start = chrono::high_resolution_clock::now( );
				process( utility );

				auto end = chrono::high_resolution_clock::now( );
				auto processing_time = end - start;
				auto delta = kMainLoopFrequency - processing_time;

				if ( delta > 0ms && should_run.load( ) ) {
					load = 0.001;
					this_thread::sleep_for( delta );
				} else setLoad( static_cast<double>( processing_time.count( ) ) );
			}

			is_running.store( false );
			ConsoleLogErr( "stopped running!" );
		}

		virtual void process( CoreUtility& utility ) = 0;
		virtual void start( CoreUtility& utility ) { };

		int GetTimeInMilliseconds( ) {
			time_point<steady_clock, sec_type> sec = time_point_cast<sec_type>( high_resolution_clock::now( ) );
			return sec.time_since_epoch( ).count( );
		}

		string GetTimeInMillisecondsString( ) {
			return to_string( GetTimeInMilliseconds( ) );
		}

		void SendLogInfo( const string& msg, const EntityId entityId, CoreUtility& utility ) {
			utility.SendLogMessage( loggerName, msg, Option<EntityId>( entityId ) );
		}

		virtual void SendLogInfo( const string& msg, CoreUtility& utility ) {
			utility.SendLogMessage( loggerName, msg, Option<EntityId>( ) );
		}

		virtual void SendLogInfo( const LogLevel level, const string& msg, CoreUtility& utility ) {
			utility.SendLogMessage( level, loggerName, msg, Option<EntityId>( ) );
		}

		virtual void SendLogInfo( const LogLevel level, const string& msg,
			const EntityId entityId, CoreUtility& utility ) {
			utility.SendLogMessage( level, loggerName, msg, Option<EntityId>( entityId ) );
		}

		void Stop( ) { should_run.store( false ); }

		void Start( ) { should_run.store( true ); }

		bool ShouldRun( ) { return should_run.load( ); }

		bool IsRunning( ) { return is_running.load( ); }

	protected:
		double CurrentTimeLoad( ) { return load; }
		void ConsoleLogErr( const string& msg ) { cerr << loggerName << " - " << msg << endl; }
		void Tick( ) { ++log_timer; }
		unsigned long long int getTime( ) { return log_timer; }
	};
}


#endif // !THREAD_BUILDER_H