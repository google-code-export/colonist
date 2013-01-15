// - prints "Starting 0.0"
// - prints "WaitAndPrint 5.0"
// - prints "Done 5.0"

print ("Starting " + Time.time);
// Start function WaitAndPrint as a coroutine
yield WaitAndPrint();
print ("Done " + Time.time);

function WaitAndPrint () {
// suspend execution for 5 seconds
yield WaitForSeconds (5);
print ("WaitAndPrint "+ Time.time);
}


function Update()
{
print("Update");
}
