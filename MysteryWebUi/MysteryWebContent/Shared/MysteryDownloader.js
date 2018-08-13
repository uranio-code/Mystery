function isContent(data) {
    return angular.isObject(data) && angular.isDefined(data.guid) && angular.isString(data.guid) && data.guid.length > 0;
}

app.factory("ContentMem", function () {
    var me = this;
    me.memory = {};

    me.parseData = function (data) {
        if (!angular.isObject(data)) return data;
        if (angular.isDefined(data.ddate))//we received a date from server
            return new Date(Date.parse(data.value));

        angular.forEach(data, function (value, key) {
            //going recursive right on the received value, this should stop recursion
            //if a guid object is represent in parent tree node
            value = me.parseData(value);
            //we replace the value with the parsed one, so dates are set.
            //yet if turn out to be a content we will replace with the one in memory
            data[key] = value;
            if (isContent(value)) {

                if (angular.isDefined(me.memory[value.guid])) {
                    //we have it already in memory
                    //therefore we extend the one we have in memory with this one
                    angular.extend(me.memory[value.guid], value);
                    //we replace the new value with the one in memory so we keep references
                    data[key] = me.memory[value.guid];
                    //as we could had override some already in memory we keep looping with the in memory object
                    //value = me.memory[value.guid];
                }
                else {
                    //welcome to the family
                    me.memory[value.guid] = value;
                }

                //if we have also the tiny guid we track by that too
                if (angular.isDefined(value.tiny_guid)) {
                    //some controllers could already had asked the content by tiny uid
                    //in that case they might be pointing a different object
                    if (angular.isDefined(me.memory[value.tiny_guid])) {
                        angular.extend(me.memory[value.tiny_guid], value);
                    }
                    else
                        //nope, nobody was tracking that yet
                        me.memory[value.tiny_guid] = value;
                }

            }

        });

        return data;
    };

    me.recostructData = function (data) {
        if (angular.isArray(data))
            return data.map(me.recostructData);

        if (!angular.isObject(data)) return data;

        //we have an object let's see if is one of ours
        //if it is we can stop otherwise we go in recursion
        if (angular.isDefined(data.guid)) {
            //we have a candidate
            if (angular.isDefined(me.memory[data.guid])) {
                //yup one of us
                return me.memory[data.guid];
            }
            else {
                //we have received an object with guid but is not in memory
                //all objects with guid shall be in memory, let's add it!
                me.parseData({ data: data });
                return me.memory[data.guid];
            }
        }

        //it is an object without guid, we go recursive on the properties
        angular.forEach(data, function (value, key) {
            data[key] = me.recostructData(value);
        });

        return data;
    };


    return me;
});

app.factory("MysteryDownloader", ['$http', 'MysteryLogin', '$rootScope', 'ContentMem', '$q', function ($http, MysteryLogin, $rootScope, ContentMem, $q) {
    var me = this;
    me.user = null;
    me.logged = false;

    me.isUnauthenticatedRespose = function (data) {
        return angular.isDefined(data) &&
            angular.isDefined(data.UnAuthorized) &&
            angular.isDefined(data.message) &&
            data.UnAuthorized &&
            data.message === "unauthenticated";
    };
    me.isUnauthorizedRespose = function (data) {
        return angular.isDefined(data) &&
            angular.isDefined(data.UnAuthorized) &&
            angular.isDefined(data.message) &&
            data.UnAuthorized &&
            data.message === "unauthorized";
    };
    me.isSuccessfullRespose = function (data) {
        if (me.isUnauthenticatedRespose(data))
            return true;//not exactly successful but nothing to worry about.
        if (me.isUnauthorizedRespose(data))
            return true;//not exactly successful but nothing to worry about.
        if (!angular.isDefined(data)) //we have no data
            return true;
        if (!angular.isDefined(data.isSuccessfull))//not mystery std answer
            return true;
        var result = data.isSuccessfull;
        if (!result)
            console.log(data.message);
        return result;
    };

    me.parseRespose = function (data) {
        //we convert json_output to output because we want the objects
        if (angular.isDefined(data) &&
                    angular.isDefined(data.json_output)) {
            data.output = angular.fromJson(data.json_output);
            delete data.json_output;
        }
        var result = {};
        result.data = data;
        result = ContentMem.parseData(result);


        if (angular.isDefined(result.data.output) && result.data.output !== null) {
            //let's see if some contents where deleted
            if (angular.isDefined(result.data.output.deleted_contents)) {
                for (var index in result.data.output.deleted_contents) {
                    $rootScope.$broadcast("content_deleted", result.data.output.deleted_contents[index]);
                };
            }
            //or new where created
            if (angular.isDefined(result.data.output.new_contents)) {
                for (var index in result.data.output.new_contents) {
                    $rootScope.$broadcast("new_content", result.data.output.new_contents[index]);
                };
            }

            //or something could be interested of a change
            if (angular.isDefined(result.data.output.changed_contents)) {
                for (var index in result.data.output.changed_contents) {
                    $rootScope.$broadcast("changed_content", result.data.output.changed_contents[index]);
                };
            }

        }


        return result.data;
    };

    me.get = function (url, callback) {
        //this is to detect an old implementation I want to get rid of
        //before we were authorzing retrive of content by guid only, that's not nice, we should avoid that.
        if (url.toString().indexOf("undefined") > 0)
            console.log(url);
        if (angular.isUndefined(callback)) callback = function () { };
        $http.get(url).then(function (response) {
            var data = response.data;
            if (!me.isSuccessfullRespose(data)) {
                console.log(url);
            };
            if (me.isUnauthenticatedRespose(data)) {
                //if we got unauthenticated we will have also to make sure we have the current user load after login
                MysteryLogin.login(function () {
                    me.get(url,
                        function (data) { me.getCurrentUser(); callback(data); });
                });
            }
           
            else {
                callback(me.parseRespose(data));
            }

        });
    };

    me.getPromise = function (url, dataProcess) {
        if (angular.isUndefined(dataProcess))
            dataProcess = function (data) { return data; };
        var deferred = $q.defer();
        me.get(url, function (data) {
            deferred.resolve(dataProcess(data));
        });
        return deferred.promise;
    };
    //this is the reverse function of parse data in the output
    //when we send something to the server we need to replace the contents with a limited copy
    //and we need to convert date into objects
    function preparePost(data) {
        if (angular.isDate(data)) {
            return { value: data.toUTCString() };
        }
        if (!angular.isObject(data)) return data;

        if (isContent(data)) {
            //we replace the contents with their cutted copy or we would go recurisive for ever
            data =  Mystery.limitedCopyContent(data);
        }

        //going recursive, looking for dates and content till the end
        angular.forEach(data, function (value, key) {
            data[key] = preparePost(value);
        });

        return data;
    }
    me.post = function (url, input, callback) {
        if (angular.isUndefined(callback)) callback = function () { };
        input = preparePost(input);

        $http.post(url, input).then(function (response) {
            var data = response.data;
            if (!me.isSuccessfullRespose(data)) {
                console.log(url);
                console.log(angular.fromJson(input));
            }
            if (me.isUnauthenticatedRespose(data)) {
                MysteryLogin.login(function () {
                    me.post(url, input, function (data) { me.getCurrentUser(); callback(data); });
                });
            }
            else {
                callback(me.parseRespose(data));
            }
        });
    };

    me.getCurrentUser = function () {
        me.get('SessionService/me', function (data) {
            var current_user = data.output;

            if (current_user !== null)
                me.logged = true;
            else
                me.logged = false;

            if (current_user !== null && me.user === null) //we just logged in
            {
                me.user = current_user;
                $rootScope.$broadcast("login", current_user);
            }
            else if (current_user === null && me.user !== null) //logged out
            {
                var logging_out = me.user;
                me.user = null;
                $rootScope.$broadcast("logout", logging_out);
            }
            else if (current_user === null && me.user === null) {
                //most likely a login failure
                //but it also could be something calling getCurrentUser for some reason
            }
            else if (current_user.guid !== me.user.guid) { //change of user
                $rootScope.$broadcast("logout", me.user);
                me.user = current_user;
                $rootScope.$broadcast("login", current_user);
            }


        });
    };

    me.getCurrentUser();

    me.logout = function () {
        me.get('SessionService/Logout', function (data) {
            me.getCurrentUser();
        });
    };


    me.contentPromises = {};

    me.handleContentDownloadAuthorization = function (requested_content, response_data, callback, unathorized_callback) {
        if (response_data.output !== null) {
            response_data.output.authorized = !response_data.UnAuthorized;
            if (response_data.output.authorized)
                callback(response_data.output);
            else
                unathorized_callback(response_data.output);
            return;
        }
        else if (response_data.UnAuthorized) {
            requested_content.authorized = false;
            unathorized_callback(requested_content);
            return;
        }
        //?!? actuall not sure we reach this, but if we do..
        callback(response_data.output);
    }
   
    //it tries load the given content which normally it is incomplete
    //if succefull the call back will be called
    //if the current user doesn't have access the unathorized_callback will be called instead
    me.loadContent = function (content, callback, unathorized_callback) {
        if (angular.isUndefined(callback))
            callback = function (content) { };
        if (angular.isUndefined(unathorized_callback))
            unathorized_callback = function (content) { };


        var guid_or_tiny = angular.isDefined(content.guid) ? content.guid : content.tiny_guid;
        var url = angular.isDefined(content.data_url) ?
                    content.data_url :
                    'ContentService/ContentView/' + content.ContentType + '/' + guid_or_tiny;

        //if we have it already we just serve it
        if (angular.isDefined(ContentMem.memory[guid_or_tiny]) && ContentMem.memory[guid_or_tiny] !== null) {
            var result = ContentMem.memory[guid_or_tiny];
            //do we have a completed version?
            if (!angular.isDefined(result.MysteryUiContentConverter) || !result.MysteryUiContentConverter) {
                //next thing is to check if we are already downloading it.
                if (angular.isDefined(me.contentPromises[guid_or_tiny]) && me.contentPromises[guid_or_tiny] !== null) {
                    //we are, let's queue the call back
                    me.contentPromises[guid_or_tiny].then(function (data) {
                        me.handleContentDownloadAuthorization(result, data, callback, unathorized_callback);
                    });
                }
                else {
                    //it is not, we shall download it,
                    var content_download_promise = me.getPromise(url);
                    content_download_promise.then(function (data) {
                        me.handleContentDownloadAuthorization(result, data, callback, unathorized_callback);
                    });
                    me.contentPromises[guid_or_tiny] = content_download_promise;
                    //when it resolve it should remove itself
                    content_download_promise.then(function () { me.contentPromises[guid_or_tiny] = null; });
                }
            }
            else {
                //it is completed, we can call immediate the callback functions
                if (angular.isUndefined(result.authorized) || result.authorized)
                    callback(result);
                else
                    unathorized_callback(result);
            }

            return result;
        }
        //if we don't have we download it, but since that will happen asynchronously
        //we return what was given and we use it as place holder to receive the data
        //it will be completed ones the service call return
        ContentMem.memory[guid_or_tiny] = content;

        me.get(url, function (data) {
            angular.extend(content, data.output);
            callback(data.output);
        });

        return content;
    };


    me.loadContents = function (contents,callback) {
        if (angular.isUndefined(callback))
            callback = function () { };

        var deferred = $q.defer();

        var to_load = contents.map(x=>x.tiny_guid);

        if(to_load.length ===0){
            deferred.resolve();
            callback();
        }

        contents.forEach(function(content){
            me.loadContent(content,function(loaded_content){
                var index = to_load.indexOf(loaded_content.tiny_guid);
                while(index>=0){
                    to_load.splice(index,1);
                    index = to_load.indexOf(loaded_content.tiny_guid);
                };
                if(to_load.length===0){
                    deferred.resolve();
                    callback();
                }
            });
        });
        return deferred.promise;
    };

    return me;
}]);