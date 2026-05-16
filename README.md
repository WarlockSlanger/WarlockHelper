# WarlockHelper
Celeste helper mod.

## Triggers:
Dash Direction Trigger: Trigger which changes the direction the Player dashes in. In Disable mode, reset to default. In Replace mode, each of the 8 directions is manually replaced (not compatible with 360 dashing). In Matrix mode, "Direction" is in the format "A,B,C,D,X,Y", which maps a direction (x,y) to (Ax+By+X,Cx+Dy+Y) (Use A and D to stretch the dash speed, B and C to skew or rotate it, and X and Y to offset it). In both modes neutral dash directions can be overriden, instead of just dashing left/right.