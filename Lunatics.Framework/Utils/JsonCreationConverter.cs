﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lunatics.Utils
{
	//public class MessageConverter : JsonCreationConverter<ConversationAPI.Message>
	//{
	//	private const string SomeOtherStuffField = "sos";

	//	protected override ConversationAPI.Message Create(Type objectType, JObject jObject)
	//	{
	//		if (FieldExists(SomeOtherStuffField, jObject))
	//		{
	//			return new ConversationAPI.DerivedMessage();
	//		}

	//		return new ConversationAPI.Message();
	//	}

	//	private bool FieldExists(string fieldName, JObject jObject)
	//	{
	//		return jObject[fieldName] != null;
	//	}
	//}
	public abstract class JsonCreationConverter<T> : JsonConverter<T>
	{
		/// <summary>
		/// Create an instance of objectType, based properties in the JSON object
		/// </summary>
		/// <param name="objectType">type of object expected</param>
		/// <param name="jObject">contents of JSON object that will be deserialized</param>
		/// <returns></returns>
		protected abstract T Create(Type objectType, JObject jObject);

		public override T ReadJson(JsonReader reader,
		                           Type objectType,
		                           T existingValue,
		                           bool hasExistingValue,
		                           JsonSerializer serializer)
		{
			// Load JObject from stream
			JObject jObject = JObject.Load(reader);

			// Create target object based on JObject
			T target = Create(objectType, jObject);

			// Populate the object properties
			serializer.Populate(jObject.CreateReader(), target);

			return target;
		}

		public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}