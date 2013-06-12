P2P system based on entropy routing and information neighborhoods. client + server

DISCLAIMER: the code is some years old and may need adjustements to work with latest .net framework.

#Roughly how it works

+ uploaded data is organized in resource rings. In this implementation each ring contains a particular file type
+ a node is connected to a set of neighbors in each ring
+ queries are propagated on relevant rings through neighbors
+ neighbors are determined based on entropy distance (nodes holding similar resources tend to be neighbors)

The idea was that by clustering nodes in a p2p network by their information distance for similar resources that your requests should resolve very quickly as most likely sources of information will be in your own neighborhood and long propagations are not necessary.
