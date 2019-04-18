//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

import Ice
import TestCommon
import PromiseKit

open class TestFactoryI: TestFactory {
    public class func create() -> TestHelper {
        return Client()
    }
}

public class Client: TestHelperI {
    public override func run(args: [String]) throws {
        let (communicator, _) = try self.initialize(args: args)
        defer {
            communicator.destroy()
        }
        let cl = try allTests(self)
        try cl.shutdown()
    }
}
