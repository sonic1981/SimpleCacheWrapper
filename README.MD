[![Build Status](https://travis-ci.org/sonic1981/SimpleCacheWrapper.svg?branch=master)](https://travis-ci.org/sonic1981/SimpleCacheWrapper)

A simple in process memory cache that uses `ConcurrentDictionary` to store objects.

Usage
-----

```
private readonly ISimpleCacheWrapper _cacheManager;

public async Task<MyClass> GetData()
{
...
}

public async Task<MyClass> GetData_Cached()
{
   return await _cacheManager.GetObjectFromCacheAsync(() => this.GetData(), "KeyToAssociateWithData", 60);
}
```