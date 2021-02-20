var OnFocusPlugin = {
    onFocus: function(link)
    {
    	GetUnity().SendMessage('WEBCONNECTOR','BrowserFocus','...');
    }
};

mergeInto(LibraryManager.library, OnFocusPlugin);