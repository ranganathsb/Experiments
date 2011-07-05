using System.Collections.Generic;
using ExpectedObjects;
using NUnit.Framework;

namespace ExpectedObjectsAssertions
{
	[TestFixture]
	public class When_retrieving_a_customer
	{
		[SetUp]
		public void Setup()
		{
			_expected = new Customer
			            	{
			            		Name = "Jane Doe",
			            		PhoneNumber = "5128651000"
			            	}.ToExpectedObject();

			_actual = new Customer
			          	{
			          		Name = "John Doe",
			          		PhoneNumber = "5128654242"
			          	};
		}

		private Customer _actual;
		private ExpectedObject _expected;

		[Test, Explicit("This correctly reports that the customer properties don't match")]
		public void Should_provide_the_expected_customer()
		{
			_expected.ShouldEqual(_actual);
		}

		[Test, Explicit("This fails unintentionally because the ExpectedObject does not override Equals")]
		public void Should_return_the_expected_customer()
		{
			Assert.That(_actual, Is.EqualTo(_expected));
		}
	}

	[TestFixture]
	public class When_retrieving_a_customer_with_address
	{
		[SetUp]
		public void Setup()
		{
			_expected = new Customer
			            	{
			            		Name = "Jane Doe",
			            		PhoneNumber = "5128651000",
			            		Address = new Address
			            		          	{
			            		          		AddressLineOne = "123 Street",
			            		          		AddressLineTwo = string.Empty,
			            		          		City = "Austin",
			            		          		State = "TX",
			            		          		Zipcode = "78717"
			            		          	}
			            	}.ToExpectedObject();

			_actual = new Customer
			          	{
			          		Name = "John Doe",
			          		PhoneNumber = "5128654242",
			          		Address = new Address
			          		          	{
			          		          		AddressLineOne = "456 Street",
			          		          		AddressLineTwo = "Apt. 3",
			          		          		City = "Waco",
			          		          		State = "TX",
			          		          		Zipcode = "76701"
			          		          	}
			          	};
		}

		private Customer _actual;
		private ExpectedObject _expected;

		[Test, Explicit("This correctly reports that the customer and address properties don't match")]
		public void Should_return_the_expected_customer()
		{
			_expected.ShouldEqual(_actual);
		}
	}

	[TestFixture]
	public class When_retrieving_a_collection_of_customers
	{
		[SetUp]
		public void Setup()
		{
			_expected = new List<Customer>
			            	{
			            		new Customer {Name = "Customer A"},
			            		new Customer {Name = "Customer B"}
			            	}.ToExpectedObject();

			_actual = new List<Customer>
			          	{
			          		new Customer {Name = "Customer A"},
			          		new Customer {Name = "Customer C"}
			          	};
		}

		private List<Customer> _actual;
		private ExpectedObject _expected;

		[Test, Explicit("This correctly reports that the lists contain elements with different properties")]
		public void Should_return_the_expected_customers()
		{
			_expected.ShouldEqual(_actual);
		}
	}

	[TestFixture]
	public class When_retrieving_a_dictionary
	{
		[SetUp]
		public void Setup()
		{
			_expected = new Dictionary<string, string> {{"key1", "value1"}};
			_actual = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}};
		}

		private IDictionary<string, string> _actual;
		private IDictionary<string, string> _expected;

		[Test, Explicit("This correctly reports that the dictionaries contain different entries")]
		public void Should_return_the_expected_dictionary()
		{
			_expected.ToExpectedObject().ShouldEqual(_actual);
		}
	}

	[TestFixture]
	public class When_retrieving_a_type_with_an_index
	{
		[SetUp]
		public void Setup()
		{
			_expected = new IndexType<int>(new List<int> {1, 2, 3, 4, 6});
			_actual = new IndexType<int>(new List<int> {1, 2, 3, 4, 5});
		}

		private IndexType<int> _actual;
		private IndexType<int> _expected;

		[Test, Explicit("This correctly reports that the items at given indices are different")]
		public void Should_return_the_expected_type()
		{
			_expected.ToExpectedObject().ShouldEqual(_actual);
		}
	}

	[TestFixture]
	public class When_matching_a_partial_customer
	{
		[SetUp]
		public void Setup()
		{
			_expected = new
			            	{
			            		Name = expectedName,
			            		Address = new
			            		          	{
			            		          		City = expectedCity
			            		          	}
			            	}.ToExpectedObject();

			_actual = new Customer
			          	{
			          		Name = "John Doe",
			          		PhoneNumber = "5128654242",
			          		Address = new Address
			          		          	{
			          		          		AddressLineOne = "456 Street",
			          		          		AddressLineTwo = "Apt. 3",
			          		          		City = "Waco",
			          		          		State = "TX",
			          		          		Zipcode = "76701"
			          		          	}
			          	};
		}

		private Customer _actual;
		private ExpectedObject _expected;
		private const string expectedName = "Jane Doe";
		private const string expectedCity = "Austin";

		[Test, Explicit("This fails with an error different from what I'd expect. Gotta ask Derek about it.")]
		public void Should_compare_all_properties_when_testing_for_equality()
		{
			_actual.Name = expectedName;
			_actual.Address.City = expectedCity;
			_expected.ShouldEqual(_actual);
		}

		[Test,
		 Explicit("This correctly reports the ExpectedObject properties that don't match the actual object's properties")]
		public void Should_have_the_correct_name_and_address_where_supplied()
		{
			_expected.ShouldMatch(_actual);
		}

		[Test]
		public void Should_not_verify_properties_that_were_not_set_up_as_expected()
		{
			_actual.Name = expectedName;
			_actual.Address.City = expectedCity;
			_expected.ShouldMatch(_actual);
		}
	}

	[TestFixture]
	public class When_testing_equivalence_or_matching_for_partially_supplied_properties
	{
		private const string ExpectedName = "Expected Name";
		private Customer _actual;
		private ExpectedObject _expected;

		[Test]
		public void Should_flag_extra_properties_on_actual_object_as_unequal()
		{
			_actual = new Customer {Name = ExpectedName, PhoneNumber = "1234567890"};
			_expected = new {Name = ExpectedName}.ToExpectedObject();
			_expected.ShouldEqual(_actual);
		}

		[Test]
		public void Should_flag_uninitialized_properties_on_actual_object_as_not_matching()
		{
			_actual = new Customer {Name = ExpectedName};
			_expected = new {Name = ExpectedName, PhoneNumber = "1234567890"}.ToExpectedObject();
			_expected.ShouldMatch(_actual);
		}

		[Test]
		public void Should_flag_uninitialized_properties_on_actual_object_as_unequal()
		{
			_actual = new Customer {Name = ExpectedName};
			_expected = new {Name = ExpectedName, PhoneNumber = "1234567890"}.ToExpectedObject();
			_expected.ShouldEqual(_actual);
		}

		[Test]
		public void Should_ignore_extra_properties_on_actual_object_when_matching()
		{
			_actual = new Customer {Name = ExpectedName, PhoneNumber = "1234567890"};
			_expected = new {Name = ExpectedName}.ToExpectedObject();
			_expected.ShouldMatch(_actual);
		}
	}

	internal class Customer
	{
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public Address Address { get; set; }
	}

	internal class Address
	{
		public string AddressLineOne { get; set; }
		public string AddressLineTwo { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zipcode { get; set; }
	}

	internal class IndexType<T>
	{
		private readonly IList<T> _ints;

		public IndexType(IList<T> ints)
		{
			_ints = ints;
		}

		public T this[int index]
		{
			get { return _ints[index]; }
		}

		public int Count
		{
			get { return _ints.Count; }
		}
	}
}