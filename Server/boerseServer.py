from flask import Flask, request
import json
import datetime
import uuid
import os
# imports for flask-dynamo
from boto.dynamodb2.fields import HashKey
from boto.dynamodb2.table import Table
from flask.ext.dynamo import Dynamo 

print(os.environ["AWS_ACCESS_KEY_ID"])
print(os.environ["AWS_SECRET_ACCESS_KEY"])

app = Flask(__name__)
# Schema definition for dynamoDB
app.config['DYNAMO_TABLES'] = [
	Table('orders', schema=[HashKey('id')]),
	Table('stocks', schema=[HashKey('id')]),
]

# init object for handling dynamoDB
dynamo = Dynamo(app)

idBoerse = 'datBoerse'
@app.route('/')
def hello():
    return "<h1 style='color:blue'>Hello There!</h1>"

@app.route('/order', methods=['PUT','GET','DELETE'])
def order():

    #check if content-type is json, if not http status 400

        if request.method == 'PUT':

            if request.headers['Content-Type'] == 'application/json':
                data = request.get_json()

                try:
                    order = {}
                    order['id'] = str(uuid.uuid4())
                    order['idStock'] = data['idStock']
                    order['amount'] = data['amount']
                    order['price'] = data['price']
                    order['type'] = data['type']
                    order['timestamp'] = str(datetime.datetime.utcnow())
                    order['idBoerse'] = idBoerse
                    order['signature'] = 'sig-optional'
                    order['idBank'] = data['idBank']
                    order['idCustomer'] = data['idCustomer']
                    txlist = []
                    order['txhistory'] = txlist
		    
                    dynamo.orders.put_item(data={
		        'id': order['id'],
			'idStock': order['idStock'],
  			'amount': order['amount'], 
                        'price': order['price'],
			'type': order['type'],
			'timestamp': order['timestamp'],
			'idBoerse': order['idBoerse'],
  			'signature': order['signature'],
                        'idBank': order['idBank'], 
			'idCustomer': order['idCustomer'], 
			'txhistory':  order['txhistory'], 
		    }) 	            
	              
                    return json.dumps(order), 200

                except Exception as error:
                    return 'Exception:\n{0}'.format(error), 500
            else:
                return 'Content of Request is not JSON',400

        elif request.method == 'GET':
            try:
                orderId = request.args.get('orderId')

		if orderId == None:
                    return 'No orderId provided',400
		
		dynorder = dynamo.orders.get_item(id=orderId)

		order = {}
                order['id'] = dynorder['id'] 
                order['idStock'] = dynorder['idStock']
                order['amount'] = dynorder['amount']
                order['price'] = dynorder['price']
                order['type'] = dynorder['type']
                order['timestamp'] = dynorder['timestamp']
                order['idBoerse'] = dynorder['idBoerse']
                order['signature'] = dynorder['signature']
                order['idBank'] = dynorder['idBank']
                order['idCustomer'] = dynorder['idCustomer']
                order['txhistory'] = dynorder['txhistory']

		return json.dumps(order)	

                else:
                    return 'Dont have this orderId in System',400

            except Exception as error:
                return 'Exception:\n{0}'.format(error),500
        elif request.method == 'DELETE':
            return 'Not implemented',200
        else:
            return 'Reques Method is not allowed here',400

@app.route('/stock', methods=['GET'])
def stock():
    try:
        stock = {}
        stock1 = {}
        stock2 = {}
        stock['id'] = 'st8822'
        stock['name'] = 'Mighty Inc.'
        stock['price'] = 6900.44
        stock['idBoerse'] = idBoerse
        stock1['id'] = 'st7890'
        stock1['name'] = 'Poppel Inc.'
        stock1['price'] = 2.57
        stock1['idBoerse'] = idBoerse
        stock2['id'] = 'st666'
        stock2['name'] = 'Luzifer GmbH'
        stock2['price'] = 666.00
        stock2['idBoerse'] = idBoerse

        stocks = [stock,stock1,stock2]

        return json.dumps(stocks),200
    except Exception as error:
        return 'Exception:\n{0}'.format(error),500
#class order(object):
#  def __init__(self, id, idStock, amount, price, type, txhistory, timestamp, idBoerse, signature, idBank, idCustomer):
#     self.id = id
#     self.idStock = idStock
#     self.amount = amount
#     self.price = price
#     self.type = type
#     self.txhistory = txhistory
#     self.timestamp = timestamp
#     self.idBoerse = idBoerse
#     self.signature = signature
#     self.idBank = idBank
#     self.idCustomer = idCustomer
#
#class stock(object):
#    def __init__(self,id,name,price,idBoerse):
#        self.id = id
#        self.name = name
#        self.price = price
#        self.idBoerse = idBoerse
#
#def createOrder():
#    return 'foo'
if __name__ == "__main__":
    app.run(host='0.0.0.0')
