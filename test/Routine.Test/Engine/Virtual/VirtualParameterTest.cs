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

        Assert.That(testing.Owner, Is.SameAs(_owner));
    }

    [Test]
    public void Parent_type_is_owner_s_parent_type()
    {
        var parentTypeMock = new Mock<IType>();
        _ownerMock.Setup(o => o.ParentType).Returns(parentTypeMock.Object);

        IParameter testing = new VirtualParameter(_owner);

        Assert.That(testing.ParentType, Is.SameAs(parentTypeMock.Object));
    }

    [Test]
    public void Name_is_required()
    {
        IParameter testing = new VirtualParameter(_owner)
            .Name.Set("virtual")
        ;

        Assert.That(testing.Name, Is.EqualTo("virtual"));

        testing = new VirtualParameter(_owner);

        Assert.That(() => { var dummy = testing.Name; }, Throws.TypeOf<ConfigurationException>());
    }

    [Test]
    public void ParameterType_is_required()
    {
        var parameterTypeMock = new Mock<IType>();

        IParameter testing = new VirtualParameter(_owner)
            .ParameterType.Set(parameterTypeMock.Object)
        ;

        Assert.That(testing.ParameterType, Is.SameAs(parameterTypeMock.Object));

        testing = new VirtualParameter(_owner);

        Assert.That(() => { var dummy = testing.ParameterType; }, Throws.TypeOf<ConfigurationException>());
    }

    [Test]
    public void Index_is_optional()
    {
        IParameter testing = new VirtualParameter(_owner)
            .Index.Set(2)
        ;

        Assert.That(testing.Index, Is.EqualTo(2));

        testing = new VirtualParameter(_owner);

        Assert.That(testing.Index, Is.EqualTo(0));
    }

    [Test]
    public void Not_supported_features()
    {
        IParameter testing = new VirtualParameter(_owner);

        Assert.That(testing.GetCustomAttributes().Length, Is.EqualTo(0));
        Assert.That(testing.IsOptional, Is.False);
        Assert.That(testing.HasDefaultValue, Is.False);
        Assert.That(testing.DefaultValue, Is.Null);
    }
}
