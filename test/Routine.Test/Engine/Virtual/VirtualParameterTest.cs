using Routine.Core.Configuration;
using Routine.Engine.Virtual;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class VirtualParameterTest : CoreTestBase
{
    #region Setup & Helpers

    private Mock<IParametric> _ownerMock;
    private IParametric _owner;

    public override void SetUp()
    {
        base.SetUp();

        _ownerMock = new();
        _owner = _ownerMock.Object;
    }

    #endregion

    [Test]
    public void Owner_is_what_is_given_as_owner()
    {
        IParameter testing = new VirtualParameter(_owner);

        Assert.AreSame(_owner, testing.Owner);
    }

    [Test]
    public void Parent_type_is_owner_s_parent_type()
    {
        var parentTypeMock = new Mock<IType>();
        _ownerMock.Setup(o => o.ParentType).Returns(parentTypeMock.Object);

        IParameter testing = new VirtualParameter(_owner);

        Assert.AreSame(parentTypeMock.Object, testing.ParentType);
    }

    [Test]
    public void Name_is_required()
    {
        IParameter testing = new VirtualParameter(_owner)
            .Name.Set("virtual")
        ;

        Assert.AreEqual("virtual", testing.Name);

        testing = new VirtualParameter(_owner);

        Assert.Throws<ConfigurationException>(() => { var dummy = testing.Name; });
    }

    [Test]
    public void ParameterType_is_required()
    {
        var parameterTypeMock = new Mock<IType>();

        IParameter testing = new VirtualParameter(_owner)
            .ParameterType.Set(parameterTypeMock.Object)
        ;

        Assert.AreSame(parameterTypeMock.Object, testing.ParameterType);

        testing = new VirtualParameter(_owner);

        Assert.Throws<ConfigurationException>(() => { var dummy = testing.ParameterType; });
    }

    [Test]
    public void Index_is_optional()
    {
        IParameter testing = new VirtualParameter(_owner)
            .Index.Set(2)
        ;

        Assert.AreEqual(2, testing.Index);

        testing = new VirtualParameter(_owner);

        Assert.AreEqual(0, testing.Index);
    }

    [Test]
    public void Not_supported_features()
    {
        IParameter testing = new VirtualParameter(_owner);

        Assert.AreEqual(0, testing.GetCustomAttributes().Length);
        Assert.IsFalse(testing.IsOptional);
        Assert.IsFalse(testing.HasDefaultValue);
        Assert.IsNull(testing.DefaultValue);
    }
}
