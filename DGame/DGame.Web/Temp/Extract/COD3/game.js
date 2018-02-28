var requestAnimationFrame = window.requestAnimationFrame ||
                window.mozRequestAnimationFrame ||
                window.webkitRequestAnimationFrame ||
                window.msRequestAnimationFrame ||
                function (callback) { setTimeout (callback, 1000 / 30); };

var canvas = document.getElementById("game-canvas");
canvas.width = 800;
canvas.height = 600;
var context = canvas.getContext("2d");

var myX=40000, myY=30000,myZ=100,myR=10000,mishkaX=0,mishkaY=0,brVragove=0,brPatroni=0,pX=[],pY=[],pZ=[],p=[],broi2=0,nachalo=true,restart=false;
var lives=3,score=0;
var vX=[],vY=[],vZ=[],vR=[],i;
var strelba=0,skorost=4;
var sniper=new Image;
sniper.src="/Game/GetFile?gameName=COD3&filename=sniper.png";
for(i=0;i<40;i++){
  vR[i]=10000;
  vX[i]=Math.floor(Math.random()*80000)-40000;
  vY[i]=Math.floor(Math.random()*60000)-30000;
  vZ[i]=Math.floor(1000+Math.random()*10000);
  brVragove++;
}
function x3D(x,z){
      return x/z+400;           
}
function y3D(y,z){
      return y/z+300;   
}
function r3D(r,z){
    return r/z;
}
function collision (p1x,p1y,r1,p2x,p2y,r2){
d=Math.sqrt((p1x-p2x)*(p1x-p2x)+(p1y-p2y)*(p1y-p2y));
var radi=r1+r2;
   if(radi>d){
       return true;
   }else{
       return false;   
   }
    
}
window.addEventListener("mousedown", function (args) {
    if(!nachalo && !restart){
    if(strelba<350){
     pX[brPatroni]=myX;
    pY[brPatroni]=myY;
    pZ[brPatroni]=myZ;
    p[brPatroni]=true;
      strelba=strelba+10;
    brPatroni++;
    }
    }
},false);
window.addEventListener("mouseup", function (args) {
   if(nachalo){
     nachalo=false;
   }
   if(restart){
     myX=40000;
     myY=30000;
     myZ=100;
     myR=10000;
     mishkaX=0;
     mishkaY=0;
     brVragove=0;
     brPatroni=0;
     broi2=0;
     restart=false;
     lives=3;
     score=0;
     strelba=0;
     skorost=4;
     for(i=0;i<40;i++){
  vR[i]=10000;
  vX[i]=Math.floor(Math.random()*80000)-40000;
  vY[i]=Math.floor(Math.random()*60000)-30000;
  vZ[i]=Math.floor(1000+Math.random()*10000);
  brVragove++;
}
   }
},false);
window.addEventListener("mousemove", function (args) {
    mishkaX=args.clientX-canvas.offsetLeft;
    mishkaY=args.clientY-canvas.offsetTop;
    if(!nachalo && !restart){
      myY=(mishkaY-300)*100;
    }
}, false);
function update() {
    if(!nachalo && !restart){
    if(lives<=0){
      restart=true;
    }
    if(score==100){
      skorost=5;
    }
    broi2++;
    if(broi2==50){
     broi2=0;
    if(strelba-10>=0){
     strelba=strelba-10;
    }
    }
     myX=(mishkaX-400)*100;
    for(i=0;i<brPatroni;i++){
      pZ[i]=pZ[i]+5;  
    }
    for(i=0;i<brVragove;i++){
      for(var i1=0;i1<brPatroni;i1++){
      if(collision(x3D(vX[i],vZ[i]),y3D(vY[i],vZ[i]),r3D(vR[i],vZ[i]),x3D(pX[i1],pZ[i1]),y3D(pY[i1],pZ[i1]),r3D(myR,pZ[i1])) && vZ[i]+(2*skorost)>pZ[i1] && vZ[i]<pZ[i1]+(2*skorost) && p[i1]){
         vR[i]=10000;
          p[i1]=false;
          score=score+1;
          vX[i]=Math.floor(Math.random()*80000)-40000;
          vY[i]=Math.floor(Math.random()*60000)-30000;
          vZ[i]=Math.floor(1000+Math.random()*10000);
      }
    }
      vZ[i]=vZ[i]-skorost;  
    }
    }
    setTimeout(update, 10); 
}
function draw() {    
    context.clearRect(0, 0, canvas.width, canvas.height);     
    context.globalAlpha = 1;
    if(!nachalo && !restart){
    for(i=0;i<brVragove;i++){
      if(vZ[i]>100 && vZ[i]<10000){
         context.beginPath();
          context.fillStyle = "rgba(255, 0, 0, 0.26)"
         context.arc(x3D(vX[i],vZ[i]), y3D(vY[i],vZ[i]),r3D(vR[i],vZ[i]),0,2*Math.PI);
         context.fill();
         context.strokeStyle = "rgb(0, 0, 0)"
         context.arc(x3D(vX[i],vZ[i]), y3D(vY[i],vZ[i]),r3D(vR[i],vZ[i]),0,2*Math.PI);
         context.stroke();
      }else{
       if(vZ[i]<100){
          lives=lives-1;
          vR[i]=10000;
          vX[i]=Math.floor(Math.random()*80000)-40000;
          vY[i]=Math.floor(Math.random()*60000)-30000;
          vZ[i]=Math.floor(1000+Math.random()*10000);
       }
    }
    }
     context.fill();
    context.fillStyle = "#ff001d"
     context.fillRect(100, 10, 350,20);
    context.fillStyle = "#f5ff00"
     context.fillRect(100, 10, strelba,20);
    for(i=0;i<brPatroni;i++){
      context.beginPath();
      if(p[i]){
      context.strokeStyle = "rgb(245, 255, 0)"
      context.arc(x3D(pX[i],pZ[i]), y3D(pY[i],pZ[i]), r3D(myR,pZ[i]), 0,2*Math.PI);
       context.stroke();
      context.fillStyle = "rgba(98, 98, 98, 0.4)";
      context.arc(x3D(pX[i],pZ[i]), y3D(pY[i],pZ[i]), r3D(myR,pZ[i]), 0,2*Math.PI);
      context.fill();
      }
    }
    context.fillStyle = "rgba(255, 0, 0, 0.41)"
    context.arc(x3D(myX,myZ), y3D(myY,myZ), r3D(myR,myZ), 0,2*Math.PI);
    context.drawImage(sniper,x3D(myX,myZ)-100, y3D(myY,myZ)-100, 200,200); 
    context.fill();
     context.fillStyle="#000";
    context.font="50px Verdana";
    context.fillText("SCORE:"+score, 270, 70);
       context.fillStyle="#dd3145";
    context.font="50px Verdana";
    context.fillText("LIVES:"+lives, 20, 70);
    }
    if(nachalo){
         context.fillStyle="#313edd";
    context.font="100px Verdana";
    context.fillText("SHOOTER", 170, 200);
       context.fillStyle="#000";
    context.font="30px Verdana";
    context.fillText("Click to continue", 300, 500);
        
    }
     if(restart){
       context.fillStyle="#000";
    context.font="50px Verdana";
    context.fillText("SCORE:"+score, 275, 200);
       context.fillStyle="#000";
    context.font="30px Verdana";
    context.fillText("Click to restart", 300, 500);
        
    }
    requestAnimationFrame(draw);       
}
update();      
draw(); 