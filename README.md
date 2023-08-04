# IrmaQuicDashboard
Project containing a dashboard for analyzing and structuring the test data retrieved when testing the QUIC connection between IRMA app and IRMA server and comparing it to TCP/TLS 1.2 

## Why this application?
For my graduation at the Open University, I analyzed the speed of data traffic between app and server by comparing the difference between certain events (app and server logs). I did this for TCP/TLS 1.2 and compared it to [QUIC](https://en.wikipedia.org/wiki/HTTP/3), the new http/3 protocol, which Google introduced some years ago and was picked up by the IETF. These tests were not performed in a lab situation, but in a real world situation: the connection of the so-called [IRMA](https://www.yivi.app/) app with the authentication server. This IRMA app (I Recall My Attributes), now called Yivi, is an app which is part of a decentralized authentiation framework based on [Idemix](https://idemix.wordpress.com/).  All the data coming from the manual tests I put in the folder `app and server logs`. The purpose of this application is fourfold:
1. These logs are bloated with logdata from app and server, so I needed a way to convert them to objects and filter the right information.
2. I needed to verify if the request and response of the api calls of the app ended up on the server.
3. A dashboard calculating and showing the differences in a table
4. A summary of all data to compare QUIC with TCP/TLS 1.2

## Technical explanation
Here follows some technical explanation as a guide through the code.

### Models
Most important model is the IrmaSession, which is a representation of a test session consisting of doing calls to the IRMA server with the IRMA app. Some of these sessions I did on WiFi, some on 3G/4G. It contains the AppLogs and the ServerLogs which are generated upon connecting to (our own modified instance of) the IRMA server. Also, there is a small upload section, where you can upload these generated logs as txt files, so hence the UploadSession model. These log contains useful entries, which occur at a certain time in the flow and are therefore usable to calculate difference between request and response, which is in turn needed to benchmark the speed of the QUIC or TCP/TLS 1.2 connection.

### Enums
These enums like AppLogEntryType categorize the logs in the .txt files as a certain type, in order to be able to calculate differences in time between the type of logs. Values like RequestIssuancePermission are technical request/response type of the IRMA server. These are at a regular place in the authentication flow, so suitable to use for calculating differences between request and response.

### Logic
The two logic classes do the actual calculation and verify if the logs are correct and not incomplete. Their functions are used as derived ([NotMapped]) fields in the Entities.

### ViewModels and Views
These are quite basic Razor Pages and their corresponding ViewModels. With the data they render the tables. In the Home/Index.cshtml there is some more information on the the exact authentication process (actually, a part of).

### Controllers and Repositories
Repositories make the connection to the local SQLite database and process the data. There is no need for remote REST API calls to external applications. As an improvement of the code, this processing could/should have been put in separate Processing or Handler classes, to offload the Repositories. Currently, this processing is done in private function within the Repository class. The Repositories themself offload the Controllers of their duties, although the DashboardController is still quite large, being the most complex view of the application.


