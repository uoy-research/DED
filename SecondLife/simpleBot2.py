import clr
import sys
import time
import string

clr.AddReferenceToFile("libsecondlife.dll")
import libsecondlife
 
# Replace credentials with the firstname, lastname and password of an SL account
credentials=['BotFirstName','BotLastName','BotPassword']
currentname=""
 
class EventTracker:
    def __init__(self):
        self.data=False
    
    def Clear(self):
        self.data=False
        
    def Set(self):
        self.data=True
        
    def Wait(self):
        while not self.data:
            time.sleep(0.1)