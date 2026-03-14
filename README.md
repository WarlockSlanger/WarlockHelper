# WarlockHelper
Celeste helper mod.
## Entities:
Dash Bumper: Bumper that makes you dash in the direction it is hit. Refer to Dash Direction Controller (Matrix) for format of "Direction" field.
Dash Direction Controller: Controller which changes the direction the Player dashes in. In Replace mode, each of the 8 directions is manually replaced. In Matrix mode, "Direction" is in the format "A,B,C,D,X,Y" or "A,B,C,D" (X and Y are 0 in this case), which maps a direction (x,y) to (Ax+By+X,Cx+Dy+Y). In both modes neutral dash directions can be overriden, instead of just dashing left/right.