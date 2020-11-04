// Copyright (c) ZeroC, Inc. All rights reserved.

// Generated by makeprops.py from file config/PropertyNames.xml, Wed Nov  4 11:11:32 2020

// IMPORTANT: Do not edit this file -- any edits made here will be lost!

#ifndef ICE_INTERNAL_PropertyNames_H
#define ICE_INTERNAL_PropertyNames_H

#include <Ice/Config.h>

namespace IceInternal
{

struct Property
{
    const char* pattern;
    bool deprecated;
    const char* deprecatedBy;

    Property(const char* n, bool d, const char* b) :
        pattern(n),
        deprecated(d),
        deprecatedBy(b)
    {
    }

    Property() :
        pattern(0),
        deprecated(false),
        deprecatedBy(0)
    {
    }

};

struct PropertyArray
{
    const Property* properties;
    const int length;

    PropertyArray(const Property* p, size_t len) :
        properties(p),
        length(static_cast<int>(len))
    {
    }
};

class PropertyNames
{
public:

    static const PropertyArray IceProps;
    static const PropertyArray IceMXProps;
    static const PropertyArray IceDiscoveryProps;
    static const PropertyArray IceLocatorDiscoveryProps;
    static const PropertyArray IceBoxProps;
    static const PropertyArray IceBoxAdminProps;
    static const PropertyArray IceBridgeProps;
    static const PropertyArray IceGridAdminProps;
    static const PropertyArray IceGridProps;
    static const PropertyArray IceSSLProps;
    static const PropertyArray IceStormAdminProps;
    static const PropertyArray IceBTProps;
    static const PropertyArray Glacier2Props;
    static const PropertyArray Glacier2CryptPermissionsVerifierProps;

    static const PropertyArray validProps[];
    static const char * clPropNames[];
};

}

#endif
