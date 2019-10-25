using System;
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// The class is the abstract interface used for all shader parameters that
    /// are added to the RenderMaterials.
    /// </summary>
    public abstract class ShaderParameter
    {
        // TODO: Destructor
        //virtual ~ShaderParameter() {}

        // TODO: public ShaderParameter(ShaderParameter other){throw new NotImplementedException();}

        /// <summary>
        /// Returns the type of the shader parameter.
        /// </summary>
        /// <returns>
        /// The type of the shader parameter.
        /// </returns>
        public abstract ShaderParamType GetParamType();

        /// <summary>
        /// Returns a copy of this ShaderParameter on the heap and returns the
        /// pointer to it. The caller has to take care of memory deallocation.
        /// </summary>
        /// <returns>
        /// A pointer to the copy of this ShaderParameter.
        /// </returns>
        public abstract ShaderParameter Clone();

        /// <summary>
        /// Checks if the ShaderParameters are equal, depending on their type
        /// and parameter.
        /// </summary>
        /// <param name="otherParameter"></param>
        /// <returns>
        /// True if the ShaderParameters are equal.
        /// </returns>
        public abstract bool Equal(ShaderParameter otherParameter);

        /// <summary>
        /// Copies the parameter value from the passed ShaderParameter to this
        /// one if the classes are of the same type.
        /// </summary>
        /// <param name="otherParameter">
        /// The ShaderParameter from which the parameter value should be taken over.
        /// </param>
        public abstract void TakeOverParameterValue(ShaderParameter otherParameter);
    }

    /// <summary>
    /// The class implements the functionality of the
    /// ShaderParameter interface for float parameters.
    /// </summary>
    public class ShaderParameterFloat : ShaderParameter
    {
        public ShaderParameterFloat(float parameterValue)
        {
            d_parameterValue = parameterValue;
        }

        //! Implementation of the shader_parameter interface
        public override ShaderParamType GetParamType()
        {
            return ShaderParamType.SPT_FLOAT;
        }

        public override ShaderParameter Clone()
        {
            throw new NotImplementedException();
            // TODO: return new ShaderParameterFloat(*this);
        }

        public override bool Equal(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        public override void TakeOverParameterValue(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The float parameter value
        /// </summary>
        public float d_parameterValue;
    }

    /// <summary>
    /// The class implements the functionality of the
    /// ShaderParameter interface for int parameters.
    /// </summary>
    public class ShaderParameterInt : ShaderParameter
    {
        public ShaderParameterInt(int parameterValue)
        {
            d_parameterValue = parameterValue;
        }

        //! Implementation of the shader_parameter interface

        public override ShaderParamType GetParamType()
        {
            return ShaderParamType.SPT_INT;
        }

        public override ShaderParameter Clone()
        {
            throw new NotImplementedException();
            // TODO: return new ShaderParameterInt(*this);
        }

        public override bool Equal(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        public override void TakeOverParameterValue(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The int parameter value
        /// </summary>
        public int d_parameterValue;
    }

    /// <summary>
    /// The class implements the functionality of the
    /// ShaderParameter interface for Texture parameters.
    /// </summary>
    public class ShaderParameterTexture : ShaderParameter
    {
        public ShaderParameterTexture(Texture parameterValue)
        {
            d_parameterValue = parameterValue;
        }

        //! Implementation of the shader_parameter interface
        public override ShaderParamType GetParamType()
        {
            return ShaderParamType.SPT_TEXTURE;
        }

        public override ShaderParameter Clone()
        {
            throw new NotImplementedException();
            // TODO: return new ShaderParameterTexture(*this);
        }

        public override bool Equal(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        public override void TakeOverParameterValue(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Texture parameter value
        /// </summary>
        public Texture d_parameterValue;
    }

    /// <summary>
    /// The class implements the functionality of the
    /// ShaderParameter interface for 4x4 matrix parameters.
    /// </summary>
    public class ShaderParameterMatrix : ShaderParameter
    {
        public ShaderParameterMatrix(Lunatics.Mathematics.Matrix parameterValue)
        {
            d_parameterValue = parameterValue;
        }

        //! Implementation of the shader_parameter interface
        public override ShaderParamType GetParamType()
        {
            return ShaderParamType.SPT_MATRIX_4X4;
        }

        public override ShaderParameter Clone()
        {
            throw new NotImplementedException();
            // return new ShaderParameterMatrix(*this);
        }

        public override bool Equal(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        public override void TakeOverParameterValue(ShaderParameter otherParameter)
        {
            throw new NotImplementedException();
        }

        public Lunatics.Mathematics.Matrix d_parameterValue;
    }

    public class ShaderParameterBindings
    {
        // TODO: Destructor
        //~ShaderParameterBindings()
        //{
        //    ShaderParameterBindings::ShaderParameterBindingsMap::iterator iter = d_shaderParameterBindings.begin();
        //    ShaderParameterBindings::ShaderParameterBindingsMap::iterator end = d_shaderParameterBindings.end();

        //    while (iter != end)
        //    {
        //        delete iter->second;
        //        ++iter;
        //    }

        //    d_shaderParameterBindings.clear();
        //}

        /// <summary>
        /// Adds a matrix shader parameter to the parameter bindings
        /// </summary>
        /// <param name="parameterName">
        /// The name of the parameter as used by the shader
        /// </param>
        /// <param name="matrix">
        /// The pointer to the matrix
        /// </param>
        public void SetParameter(string parameterName, ref Lunatics.Mathematics.Matrix matrix)
        {
            var shaderParam = GetParameter(parameterName);
            if (shaderParam != null && (shaderParam.GetParamType() == ShaderParamType.SPT_MATRIX_4X4))
                ((ShaderParameterMatrix) shaderParam).d_parameterValue = matrix;
            else
                SetNewParameter(parameterName, new ShaderParameterMatrix(matrix));
        }

        /// <summary>
        /// Adds a texture shader parameter to the parameter bindings
        /// </summary>
        /// <param name="parameterName">
        /// The name of the parameter as used by the shader
        /// </param>
        /// <param name="texture">
        /// The pointer to the CEGUI::Texture
        /// </param>
        public void SetParameter(string parameterName, Texture texture)
        {
            var shaderParam = GetParameter(parameterName);
            if (shaderParam != null && (shaderParam.GetParamType() == ShaderParamType.SPT_TEXTURE))
                ((ShaderParameterTexture) shaderParam).d_parameterValue = texture;
            else
                SetNewParameter(parameterName, new ShaderParameterTexture(texture));
        }

        /// <summary>
        /// Adds a float shader parameter to the parameter bindings
        /// </summary>
        /// <param name="parameterName">
        /// The name of the parameter as used by the shader
        /// </param>
        /// <param name="fvalue">
        /// The value of the float parameter
        /// </param>
        public void SetParameter(string parameterName, float fvalue)
        {
            var shaderParam = GetParameter(parameterName);
            if (shaderParam != null && (shaderParam.GetParamType() == ShaderParamType.SPT_FLOAT))
                ((ShaderParameterFloat) shaderParam).d_parameterValue = fvalue;
            else
                SetNewParameter(parameterName, new ShaderParameterFloat(fvalue));
        }

        /// <summary>
        /// Returns a pointer to the shader_parameter with the specified parameter name
        /// </summary>
        /// <param name="parameterName">
        /// The name of the parameter as used by the shader
        /// </param>
        /// <returns>
        /// The pointer to the required shader_parameter. Will return 0 if a parameter
        /// with the specified name was not set.
        /// </returns>
        public ShaderParameter GetParameter(string parameterName)
        {
            return d_shaderParameterBindings.ContainsKey(parameterName)
                           ? d_shaderParameterBindings[parameterName]
                           : null;
        }

        /// <summary>
        /// Sets the shader_parameter in the map to 0, which means that the shader parameter
        /// will remain unchanged during rendering
        /// </summary>
        /// <param name="parameter_name">
        /// The name of the parameter as used by the shader
        /// </param>
        public void RemoveParameter(string parameter_name)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, ShaderParameter> GetShaderParameterBindings()
        {
            return d_shaderParameterBindings;
        }

        /// <summary>
        /// Adds a new shader parameter to the parameter bindings. If an old one exists it will be
        /// deleted and replaced by the new one.
        /// </summary>
        /// <param name="parameter_name">
        /// The name of the parameter as used by the shader
        /// </param>
        /// <param name="shader_parameter">
        /// The pointer to the shader_parameter
        /// </param>
        protected void SetNewParameter(string parameter_name, ShaderParameter shader_parameter)
        {
            if (d_shaderParameterBindings.ContainsKey(parameter_name))
            {
                // TODO: d_shaderParameterBindings[parameter_name].Dispose(); // delete found_iterator->second;
                d_shaderParameterBindings[parameter_name] = shader_parameter;
            }
            else
            {
                d_shaderParameterBindings.Add(parameter_name, shader_parameter);
            }
        }

        // typedef std::map<std::string, ShaderParameter*> ShaderParameterBindingsMap;

        //! Map of the names of the shader parameter and the respective shader parameter value
        protected IDictionary<string, ShaderParameter> d_shaderParameterBindings = new Dictionary<string, ShaderParameter>();

    }
}