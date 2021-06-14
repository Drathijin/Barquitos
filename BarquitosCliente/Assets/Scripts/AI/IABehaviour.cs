using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IABehaviour : MonoBehaviour
{
  public Fleet fleet_; //Grid of buttons
  protected System.Random generator_;

    //Values between 1 or 0
    double centerPriority=0.5;
    double closerPriority=0.5;
    double horizontalPriority=0.5;
  
  private void Awake() {
    generator_ = new System.Random();
  }

  public virtual AttackData Attack() {
    return new AttackData();
  }
  protected virtual Fleet SelectTarget(){return new Fleet();}
  
  private bool checkPos(Fleet myFleet, int x, int y, int length, bool horizontal)
  {
    int i=0;
    int hx = horizontal ? 1 :0;
    int hy = !horizontal ? 1 :0;
    while(i<length)
    {
      int nx, ny;
      nx = x+ (length-i)*hx;
      ny = y+ (length-i)*hy;
      if(nx < 0 && nx>=10)
        return false;
      if(ny < 0 && ny>=10)
        return false;
      // if(myFleet.GetGrid().GetPos(nx,ny).Data().Ship())
      //   return false;
        i++;
    }
    return true;
  }
  private void lookForPosition(ref int x, ref int y, int length,Fleet myFleet, bool close, bool horizontal, bool center)
  {
    int targetX, targetY;
    if(close && center || center)
    {
      targetX = 4;
      targetY = 4;
    }
    else if(close && y>= 0 && x>=0)
    {
      targetX = x;
      targetY = y;
    }
    else
    {
      targetX = generator_.Next(0,9);
      targetY = generator_.Next(0,9);
    }
    int bestX =4, bestY = 4, actualBest=1000;
    for(int i = 0; i<10; i++)
      for(int j = 0; j<10; j++)
      {
        int actual = Mathf.Abs(targetX-j)+Mathf.Abs(targetY-i);
        if(checkPos(myFleet, i,j,length,horizontal) && actual<actualBest)
        {
          bestX = j;
          bestY = i;
          actualBest = actual;
        }
      }
    x = bestX;
    y = bestY;
  }
  public void Setup(Fleet myFleet)
  {
    int lastX=-1, lastY=-1;
    List<int> ships = new List<int>();
    ships.Add(2);
    ships.Add(3);
    ships.Add(3);
    ships.Add(4);
    ships.Add(5);

    bool center,horizontal, close;
    foreach (var shipLength in ships)
    {
      center      = generator_.NextDouble()<=centerPriority;
      horizontal  = generator_.NextDouble()<=horizontalPriority;
      close       = generator_.NextDouble()<=closerPriority;
      lookForPosition(ref lastX, ref lastY, shipLength, myFleet,close,horizontal, center);
      for(int i=0;i<shipLength;i++)
        myFleet.GetGrid().GetPos(lastX+(i*(horizontal?1:0)), lastY+(i*(horizontal?0:1))).SetShip(true);
    }
  }

}
