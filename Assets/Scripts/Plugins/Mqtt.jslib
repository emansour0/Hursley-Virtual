var MqttPlugin = {

    connectMqtt : function (connectionId_param, host_param, username_param, password_param, path_param, topic_param, useSSL_param, cleanSession_param, port, onConnectCallback, onMessageCallback) {

        connectionId = Pointer_stringify(connectionId_param);
        host = Pointer_stringify(host_param);
        username = Pointer_stringify(username_param);
        password = Pointer_stringify(password_param);
        path = Pointer_stringify(path_param);
        topic = Pointer_stringify(topic_param);

        client = new Paho.MQTT.Client(host, port, path);

        var options = {
            cleanSession: cleanSession_param == 1 ? true : false,
            useSSL: useSSL_param == 1 ? true : false,
            userName: username,
            password: password, 
            onSuccess: function()
            {
                console.log(connectionId + " connected to MQTT broker successfully");
                var buffer = getPtrFromString(connectionId);
                Runtime.dynCall('vi', onConnectCallback, [buffer]);
                
                client.subscribe(topic, {qos:0});

                function getPtrFromString (str) {
                    var buffer = _malloc(lengthBytesUTF8(str) + 1);
                    writeStringToMemory(str, buffer);
                    return buffer;
                }
            },

            onFailure: function () {
                console.log(connectionId + " failed to connect to MQTT broker")
            }
        };

        client.onMessageArrived = function(message) {
            console.log("Message Received: " + message.payloadString);
            var buffer = getPtrFromString(connectionId + ':' + message.destinationName + ':' + message.payloadString);

            Runtime.dynCall('vi', onMessageCallback, [buffer]);

            function getPtrFromString (str) {
                var buffer = _malloc(lengthBytesUTF8(str) + 1);
                writeStringToMemory(str, buffer);
                return buffer;
            }
        }

        client.connect(options);

    },
};
mergeInto(LibraryManager.library, MqttPlugin)