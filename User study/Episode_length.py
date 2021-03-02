import sys, os
import numpy as np
from Util.filehandling import copy_files, rename_files, write, read_file
import datetime
from datetime import timedelta

ids = { }


def get_id( v ):
    if v not in ids:
        ids[v] = len( ids )
    return ids[v]


class Episode:
    def __init__( self ):
        self.lines = []
        self.times = []
        self.trail = []
        self.start = None
        self.end = None
        self.length = None

    def __str__( self ):
        return 'Episode( {}, {} - {}, lines: {}, times: {}, trail: {} )'.format( self.length or '', self.start or '',
                                                                                 self.end or '',
                                                                                 len( self.lines ), len( self.times ),
                                                                                 len( self.trail ) )

    def add_start( self, v ):
        self.start = self.get_date( v )

    def levenshtein( seq1, seq2 ):
        size_x = len( seq1 ) + 1
        size_y = len( seq2 ) + 1
        matrix = np.zeros( (size_x, size_y) )
        for x in range( size_x ):
            matrix[x, 0] = x
        for y in range( size_y ):
            matrix[0, y] = y

        for x in range( 1, size_x ):
            for y in range( 1, size_y ):
                if seq1[x - 1] == seq2[y - 1]:
                    matrix[x, y] = min(
                        matrix[x - 1, y] + 1,
                        matrix[x - 1, y - 1],
                        matrix[x, y - 1] + 1
                        )
                else:
                    matrix[x, y] = min(
                        matrix[x - 1, y] + 1,
                        matrix[x - 1, y - 1] + 1,
                        matrix[x, y - 1] + 1
                        )
        print( matrix )
        return (matrix[size_x - 1, size_y - 1])

    def add_end( self ):
        v = self.lines[-1]
        self.end = self.get_date( v )
        if self.start:
            self.length = self.end - self.start

    def get_date( self, v ):
        return datetime.datetime.strptime( '{} {}'.format( *v.split( )[:2] ), '%Y-%m-%d %H:%M:%S,%f' )

    def add_effective_action( self, v ):
        vv = v.strip( ).split( ')' )[1:]
        vvv = get_id( tuple( vv ) )
        self.trail.append( vvv )

    def add_line( self, v ):
        self.lines.append( v )
        # if 'getRandomAction' in v:
        #     self.add_effective_action( v )
        if 'Reading prims.cix' in v:
            self.add_start( v )
        if 'FindSpeechGoalsForSelfAndSenderGoal' in v:
            if len( self.times ) and len( self.times[-1] ) == 1: return
            self.times.append( [self.get_date( v )] )
        if "Current action 'say'" in v:
            if len( self.times ):
                d = self.get_date( v )
                if d == self.times[-1][0]: return
                t = d - self.times[-1][0]
                self.times[-1].append( [self.get_date( v ), t] )


class Episodes:
    def __init__( self ):
        self.all = []
        self.valid = []

    def write_file( self, lines, path ):
        w = open( path, "w", encoding="Latin-1" )
        w.writelines( lines )
        w.close( )

    def find_calc_times( self, in_path, speech_path, episode_path, drama_effects, start, start_response, end_response ):
        r = open( in_path, "r", encoding="Latin-1" )
        e = None
        deltaM = timedelta( minutes=30 )
        deltaH = timedelta( hours=2 )
        for l in r.readlines( ):
            if 'Reading prims.cix' in l:
                if e:
                    e.add_end( )
                    self.all.append( e )
                    if deltaM < e.length and deltaH > e.length:
                        self.valid.append( e )
                e = Episode( )
            if e:
                e.add_line( l )
        r.close( )
        for e in self.valid:
            print('{} & {} & {} \\\\\\hline'.format( e.length, e.start, e.end ))


    def time_calc( self, _to, _from ):
        e = Episode( )
        e.lines = [_to]
        e.add_start( _from )
        e.add_end( )
        print( e )


def main( ):
    print( os.path.abspath( os.path.pardir ) )
    eps = Episodes( )
    # faults = [['2010-11-01 22:54:33,765', '2010-11-01 22:06:23,375000'],
    #           ['2010-11-03 23:55:44,953', '2010-11-03 23:38:54,359000'],
    #           ['2011-01-20 14:39:49,125', '2011-01-20 13:06:15,984000'],
    #           ['2011-01-20 14:39:49,125', '2011-01-20 13:06:15,984000'],
    #           ['2011-01-24 22:56:48,406', '2011-01-24 14:24:23,718'],
    #           ['2011-01-24 10:30:05,906', '2011-01-24 10:10:36,015'],
    #           ['2011-04-22 11:35:52,421', '2011-04-22 10:43:38,203'],
    #           ['2011-05-23 13:18:46,015','2011-05-23 13:13:35,812000'],
    #           ['2011-05-31 10:02:36,437000','2011-05-31 09:49:57,468']
    #           ]
    # for fault in faults:
    #     eps.time_calc( *fault )
    eps.find_calc_times( 'Data/DED_logs/ActorLogWeb.txt',
                         'Data/DED_logs/ActorLogWebSpeechTimesFull.txt',
                         'Data/DED_logs/ActorLogWebValidEpisodeTimesFull.txt',
                         'Data/DED_logs/ActorLogWebDramaEffects.txt', start='Reading prims.cix',
                         start_response='FindSpeechGoalsForSelfAndSenderGoal',
                         end_response="Current action 'say'" )


if __name__ == '__main__':
    main( )
    sys.exit( 0 )
