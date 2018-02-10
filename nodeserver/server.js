var WebSocketServer = require('ws').Server
  , wss = new WebSocketServer({ port: 54545 });
var fs = require('fs');

console.log("Waiting...")

wss.on('connection', function connection(ws) {
    console.log('connection');

    // var callflag = false;

    // // Use to check if we haven't received data in the allotted time save
    // // the file and disconnect..
    // setTimeout(function(arg) {
    //     if (callflag === false) {
    //         stream.close();
    //         ws.close();
    //     }
    // }, 5000);

    var stream = fs.createWriteStream("in.wav");
    var times = 0;
    var done = false;
    ws.on('message', function incoming(message) {
        
        //var cf = callflag;
        console.log('received: %s', message);
        //cf = true;
        // collect the data into a wav file complete with header..
        if (!done)
            stream.write(message);    
        if (times++ > 10) {
            stream.close();
            done = true;
        }
    });
});