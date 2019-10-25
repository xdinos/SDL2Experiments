#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

namespace SharpCEGui.Base
{
    /// <summary>
    /// Dimension type that represents the result of an operation performed on
    /// two other dimension values. Implements BaseDim interface.
    /// </summary>
    public class OperatorDim : BaseDim
    {
        public OperatorDim()
        {
            d_left = null;
            d_right = null;
            d_op = DimensionOperator.Noop;
        }

        public OperatorDim(DimensionOperator op)
        {
            d_left = null;
            d_right = null;
            d_op = op;
        }

        public OperatorDim(DimensionOperator op, BaseDim left, BaseDim right)
        {
            d_left = left != null ? left.Clone() : null;
            d_right = right != null ? right.Clone() : null;
            d_op = op;
        }

        // TODO: ...
        //~OperatorDim()
        //{
        //    CEGUI_DELETE_AO d_right;
        //    CEGUI_DELETE_AO d_left;
        //}

        //! set the left hand side operand (passed object is cloned)
        public void SetLeftOperand(BaseDim operand)
        {
            // TODO: CEGUI_DELETE_AO d_left;
            d_left = operand!=null ? operand.Clone() : null;
        }

        //! return pointer to the left hand side operand
        public BaseDim GetLeftOperand()
        {
            return d_left;
        }

        //! set the right hand side operand (passed object is cloned)
        public void SetRightOperand(BaseDim operand)
        {
            // TODO: CEGUI_DELETE_AO d_right;
            d_right = operand != null ? operand.Clone() : null;
        }

        //! return pointer to the right hand side operand
        public BaseDim GetRightOperand(){
            return d_right;
        }

        //! Set the operation to be performed
        public void SetOperator(DimensionOperator op){
            d_op = op;
        }

        //! Get the current operation that will be performed
        public DimensionOperator GetOperator(){
            return d_op;
        }

        //! helper to set the next free operand, will throw after 2 are set
        public void SetNextOperand(BaseDim operand)
        {
            if (d_left == null)
                d_left = operand != null ? operand.Clone() : null;
            else if (d_right == null)
                d_right = operand != null ? operand.Clone() : null;
            else
                throw new InvalidRequestException("Both operands are already set.");
        }

        // Implementation of the base class interface
        public override float GetValue(Window wnd)
        {
            var lval = d_left != null ? d_left.GetValue(wnd) : 0.0f;
            var rval = d_right != null ? d_right.GetValue(wnd) : 0.0f;

            return GetValueImpl(lval, rval);
        }

        public override float GetValue(Window wnd, Rectf container)
        {
            var lval = d_left != null ? d_left.GetValue(wnd, container) : 0.0f;
            var rval = d_right != null ? d_right.GetValue(wnd, container) : 0.0f;

            return GetValueImpl(lval, rval);
        }

        public override BaseDim Clone()
        {
            return new OperatorDim(d_op, d_left, d_right);
        }

        protected float GetValueImpl(float lval, float rval)
        {
            switch (d_op)
            {
                case DimensionOperator.Noop:
                    return 0.0f;

                case DimensionOperator.Add:
                    return lval + rval;

                case DimensionOperator.Subtract:
                    return lval - rval;

                case DimensionOperator.Multiply:
                    return lval*rval;

                    // divide by zero returns zero.  Not 100% correct but is better than the
                    // alternatives in the majority of cases where LookNFeels are concerned.
                case DimensionOperator.Divide:
                    return rval == 0.0f ? rval : lval/rval;

                default:
                    throw new InvalidRequestException("Unknown DimensionOperator value.");
            }
        }

        // Implementation of the base class interface
        public override void WriteXmlToStream(XMLSerializer xmlStream)
        {
            WriteXmlElementNameImpl(xmlStream);
            WriteXmlElementAttributesImpl(xmlStream);

            if (d_left != null)
                d_left.WriteXmlToStream(xmlStream);

            if (d_right != null)
                d_right.WriteXmlToStream(xmlStream);

            xmlStream.CloseTag();
        }

        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream){
            xmlStream.OpenTag("OperatorDim");
        }

        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream){
            xmlStream.Attribute("op", d_op.ToString());
        }

        protected BaseDim d_left;
        protected BaseDim d_right;
        protected DimensionOperator d_op;
    };
}