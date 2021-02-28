from scipy import stats
import numpy as np
from collections import defaultdict
import matplotlib.pyplot as plt
import csv, sys

np.random.seed( 0 )


class UserStudy:
    def __init__( self ):
        self.characters = self.read_characters( )
        self.coherent_data = self.read_data( 'coherent' )
        self.timely_data = self.read_data( 'timely' )
        self.believable_data = { }
        self.believable_character_data = { }
        self.traits_data = { }
        self.against_hypothesis = { }
        self.read_believable_character( )
        self.read_traits( )
        self.generate_against_hypothesis( [_ for _ in range( 5 )], 100, [0, 1] )

    def read_characters( self ):
        with open( '../Data/UserEvaluation/characters.csv', newline='' ) as csvfile:
            return { _[0]: _[1] for _ in csv.reader( csvfile, delimiter=',' ) if _ }

    def read_demographics( self, type ):
        with open( '../Data/UserEvaluation/{}.csv'.format( type ), newline='' ) as csvfile:
            data = { _[0]: int( _[1] ) for _ in csv.reader( csvfile, delimiter=',' ) if _ and type.title( ) not in _ }
            N = sum( [_ for _ in data.values( )] )
            return { k: v / N * 100 for k, v in data.items( ) }

    def read_data( self, type ):
        with open( '../Data/UserEvaluation/{}.csv'.format( type ), newline='' ) as csvfile:
            reader = [_ for _ in csv.reader( csvfile, delimiter=',' ) if _]
            flat = []
            data = { }
            for i, (k, v) in enumerate( reader[1:] ):
                flat += [i] * int( v )
                data[k] = int( v )
            N = sum( data.values( ) )
            percentage = { k: v / N * 100 for k, v in data.items( ) }
            return percentage, flat

    def read_believable_character( self ):
        with open( '../Data/UserEvaluation/believable_character.csv', newline='' ) as csvfile:
            reader = [_ for _ in csv.reader( csvfile, delimiter=',' ) if _]
            data = defaultdict( lambda: 0 )
            labels = reader[0][1:]
            total_flat = []
            results = defaultdict( lambda: [{ }, [], labels] )
            for id, *values in reader[1:]:
                flat = []
                for i, v in enumerate( values[:-1] ):
                    data[labels[i]] += int( v )
                    results[id][0][labels[i]] = int( v )
                    flat += [i] * int( v )
                total_flat += flat
                results[id][1] = flat
            N = sum( data.values( ) )
            persentage = { k: v / N * 100 for k, v in data.items( ) }
            for k, v in results.items( ):
                N = sum( v[0].values( ) )
                v[0] = { k: v / N * 100 for k, v in v[0].items( ) }
            self.believable_data = [persentage, flat]
            self.believable_character_data = results

    def read_traits( self ):
        results = defaultdict( lambda: { } )
        new_csv = []
        with open( '../Data/UserEvaluation/traits.csv', newline='' ) as csvfile:
            reader = [_ for _ in csv.reader( csvfile, delimiter=',' ) if _]
            labels = reader[0][2:][:-1]
            new_csv.append( reader[0] )
            for id, trait, *values in reader[1:]:
                data = { k: int( v ) for k, v in zip( labels, values ) }
                N = sum( data.values( ) )
                persentage = { k: v / N * 100 for k, v in data.items( ) }
                flat = []
                for i, v in enumerate( values[:-1] ):
                    flat += [i] * int( v )
                results[id][int( trait )] = [persentage, flat]

        self.traits_data = results

    def plot( self, label, factors, values, x_label, y_label, colour='b' ):
        fig, ax = plt.subplots( )
        ax.bar( factors, values, width=0.25, color=colour )
        ax.set_xlabel( x_label )
        ax.set_ylabel( y_label )
        plt.ylim( [0, 100] )
        plt.title( label )
        plt.savefig( label + '.png', format='PNG' )
        plt.show( )

    def generate_against_hypothesis( self, labels, size, centres ):
        h = [sorted( [max( int( round( _ ) ), 0 ) for _ in np.random.normal( loc=i, scale=.5, size=size )] ) for i in
             centres]

        sum_h = []
        for i, _ in enumerate( h ):
            c = { l: _.count( i ) / len( _ ) * 100 for i, l in enumerate( labels ) }
            self.plot( label='Null hypothesis {}'.format( i ), factors=labels, values=c.values( ), x_label='',
                       y_label='%', colour='r' )
            sum_h.append( c )

        self.against_hypothesis = sum_h, h

    def all_stats( self ):
        self.stats( 'timely', self.timely_data )
        self.stats( 'coherent', self.coherent_data )
        self.stats( 'believable', self.believable_data )
        for k, v in self.believable_character_data.items( ):
            self.stats( 'believable {}'.format( self.characters[k] ), v )

    def traits( self ):
        self.generate_against_hypothesis( [_ for _ in range( 5 )], 100, range( 5 ) )
        self.trait_labels = [
            ['Reserved', 'Warm'],
            ['Reactive', 'Emotionally stable'],
            ['Deferential', 'Dominant'],
            ['Serious', 'Lively'],
            ['Expedient', 'Rule-conscious'],
            ['Utilitarian', 'Sensitive'],
            ['Relaxed', 'Tense'],
            ]
        for c, traits in self.traits_data.items( ):
            for t, v in traits.items( ):
                self.trait_alignment( t, self.characters[c], v )

    def fmt( self, n, n_decimels=5 ):
        return '{}:.{}f{}'.format( '{', n_decimels, '}' ).format( n, )

    def stats( self, name, data ):
        self.plot( label=name.title( ), factors=data[0].keys( ), values=data[0].values( ), x_label='', y_label='%' )
        sum_h, h = self.against_hypothesis
        for i, (s, h) in enumerate( zip( sum_h, h ) ):
            t, p = stats.ttest_ind( h, data[1] )
            print( '{} & {} & {} & {} & {} \\\\\\hline'.format( name.title( ), (list( data[0].keys( ) )[i]).lower( ),
                                                                self.fmt( t ), self.fmt( p ), p < .05 ) )

    def trait_alignment( self, trait, name, data ):
        self.plot( label='{} - {} {}'.format( *self.trait_labels[trait], name.title( ) ), factors=data[0].keys( ),
                   values=data[0].values( ), x_label='', y_label='%' )
        sum_h, h = self.against_hypothesis
        for i, (s, h) in enumerate( zip( sum_h, h ) ):
            t, p = stats.ttest_ind( h, data[1] )
            if p > 0.001 and i != 2:
                t_name = self.trait_labels[trait][0] if i in [0, 1] else self.trait_labels[trait][1]
                print( '{} & {} & {} & {}  \\\\\\hline'.format( name.title( ), t_name,
                                                                self.fmt( t ), self.fmt( p ) ) )

    def demographics( self ):
        data = self.read_demographics( 'age' )
        self.plot( label='Age', factors=data.keys( ), values=data.values( ), x_label='Years', y_label='%' )
        data = self.read_demographics( 'fps' )
        self.plot( label='FPS', factors=data.keys( ), values=data.values( ), x_label='Years', y_label='%' )
        data = self.read_demographics( 'rpg' )
        self.plot( label='RPG', factors=data.keys( ), values=data.values( ), x_label='Years', y_label='%' )


study = UserStudy( )
study.all_stats( )
study.traits( )
study.demographics( )
