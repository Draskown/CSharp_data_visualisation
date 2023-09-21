# UDP client-server data collection

Server generates four data streams with different correlations.

<center><img src="./Images/Server_main.png" height="100" width="300"/></center>

Client accepts that data and visualises it with an access of tweaking the ends of plots and their scales.

<center><img src="./Images/Client_main.png" height="400" width="600"/></center>

- `XScale` sets a scale for X axis.
- `YScale` sets a scale for Y axis.
- `Thinning` sets a sampling rate (the more the less points will be received).
- `Max Time Span` sets the upper bound for the time axis.
- `Min Time Span` sets the lower bound for the time axis.
- `Saves` the whole plot in the .png format.