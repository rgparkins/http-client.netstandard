# Common HttpClient

This project tries to facilitate the problems of monitoring and finding which network connections fhave failed 
when running in a microservice infrastructure

Before we use service mesh the network layer and communiocation is a 3rd class citizen. We need common logging to 
find out where our problems lie when communication between services.

This client in tandem with the Prometheus.NetStandard library can out of the box create you your metrics for free.

Currently supported:

- client_http_response_time_milliseconds
- client_http_response_size_bytes
- client_http_request_size_bytes

These metrics each have target and source service names as labels in prometheus and these can be logged to console or
 at time or writing a log4net appender
 
## Out of the box

This readme will be updated but for now please see the tests for examples
 

