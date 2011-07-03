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
}