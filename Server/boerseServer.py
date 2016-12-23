from flask import Flask, request
import json
import datetime
import uuid
import os
# imports for flask-dynamo
from boto.dynamodb2.fields import HashKey,RangeKey
from boto.dynamodb2.table import Table
from flask.ext.dynamo import Dynamo

from dateutil.parser import parse
from dateutil.tz import gettz

print(os.environ["AWS_ACCESS_KEY_ID"])
print(os.environ["AWS_SECRET_ACCESS_KEY"])

app = Flask(__name__)
# Schema definition for dynamoDB
app.config['DYNAMO_TABLES'] = [
        Table('orders', schema=[HashKey('id')]),
        Table('stocks', schema=[HashKey('id')]),
        Table('priceHistory', schema=[HashKey('id'),RangeKey('time')]),
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
                    order['price'] = str(data['price'])
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
                elif orderId == '':
                    dynorders = dynamo.orders.scan()
                    orders = []
                    for dynorder in dynorders:
                        order = {}
                        order['id'] = dynorder['id']
                        order['idStock'] = dynorder['idStock']
                        order['amount'] = int(dynorder['amount'])
                        order['price'] = float(dynorder['price'])
                        order['type'] = dynorder['type']
                        order['timestamp'] = dynorder['timestamp']
                        order['idBoerse'] = dynorder['idBoerse']
                        order['signature'] = dynorder['signature']
                        order['idBank'] = dynorder['idBank']
                        order['idCustomer'] = dynorder['idCustomer']
                        txhistory = []
                        if dynorder['txhistory'] != None:
                            for trans in dynorder['txhistory']:
                                hist = {}
                                hist['amount'] = int(trans[0])
                                hist['price'] = float(trans[1])
                                txhistory.append(hist)
                        order['txhistory'] = txhistory
                        orders.append(order)

                    return json.dumps(orders)

                else:
                    dynorder = dynamo.orders.get_item(id=orderId)

                    order = {}
                    order['id'] = dynorder['id']
                    order['idStock'] = dynorder['idStock']
                    order['amount'] = int(dynorder['amount'])
                    order['price'] = float(dynorder['price'])
                    order['type'] = dynorder['type']
                    order['timestamp'] = dynorder['timestamp']
                    order['idBoerse'] = dynorder['idBoerse']
                    order['signature'] = dynorder['signature']
                    order['idBank'] = dynorder['idBank']
                    order['idCustomer'] = dynorder['idCustomer']
                    txhistory = []
                    if dynorder['txhistory'] != None:
                        for trans in dynorder['txhistory']:
                            hist = {}
                            hist['amount'] = int(trans[0])
                            hist['price'] = float(trans[1])
                            txhistory.append(hist)

                    order['txhistory'] = txhistory


                    return json.dumps(order), 200

            except Exception as error:
                return 'Exception:\n{0}'.format(error),500

        elif request.method == 'DELETE':
            try:
                orderId = request.args.get('orderId')

                if orderId == None or  orderId == '':
                    return 'No orderId provided',400

                dynorder = dynamo.orders.get_item(id=orderId)

                dynorder['amount'] = 0
                if dynorder.save(overwrite=True):
                    return 'success', 200
                else:
                    return 'fail', 400
            except Exception as error:
                return 'Exception:\n{0}'.format(error),500
        else:
            return 'Reques Method is not allowed here',400

@app.route('/stock', methods=['GET'])
def stock():
    try:
        stocks = []
        dynstocks = dynamo.stocks.scan()
        for dynstock in dynstocks:
            stock={}
            stock['id'] = dynstock['id']
            stock['name'] = dynstock['name']
            stock['price'] = float(dynstock['price'])
            stock['idBoerse'] = dynstock['idBoerse']

            stocks.append(stock)

        return json.dumps(stocks),200
    except Exception as error:
        return 'Exception:\n{0}'.format(error),500
@app.route('/trade', methods=['GET'])
def trade():
    try:
        dynstocks = dynamo.stocks.scan()
        dynorders = dynamo.orders.scan()

        for stock in dynstocks:
            newStockPrice = calcExchange(stock['id'], list(dynorders))
            print(newStockPrice)
            tempstock = dynamo.stocks.get_item(id=stock['id'])
            tempstock['price'] = str(newStockPrice)
            tempstock.save()

            dynamo.priceHistory.put_item(data={
                'id': stock['id'],
                'price': str(newStockPrice),
                'time': str(datetime.datetime.utcnow())
            })
        return 'Trade went good!',200
    except Exception as error:
        return 'Exception:\n{0}'.format(error),500


def calcExchange(idStock, orders):
    prices = []
    finalPrice = 0.0
    maxVolume = 0

    # get all available prices
    for order in orders:
        if order['idStock'] == idStock:
            prices.append(float(order['price']))

    # determine with which price the most trade volume is generated
    print('-- PRICE EVALUATION --')
    for price in set(prices):
         buyCount = 0
         sellCount = 0
         for order in orders:
             if (order['type'] == 'sell' and float(order['price']) <= price and order['idStock'] == idStock):
                 sellCount += order['amount']
             elif (order['type'] == 'buy' and float(order['price']) >= price and order['idStock'] == idStock):
                 buyCount += order['amount']
         if sellCount > buyCount:
             volume = buyCount
         elif sellCount < buyCount:
             volume = sellCount
         else:
             volume = 0

         if volume > maxVolume:
             maxVolume = volume
             finalPrice = price
         elif volume == maxVolume and price < finalPrice:
             finalPrice = price

         print('price: {} -- buy: {} -- sell: {} -- volume: {}'.format(price,buyCount,sellCount,volume))

    if finalPrice == 0:
        print('-- NO TRADE because no orders match --')
        return float(dynamo.stocks.get_item(id=idStock)['price'])
    # trade with new price
    ordersSorted = sorted(orders, key=getkey)
    sellOrders = []
    buyOrders = []
    sellAmount = 0
    buyAmount = 0

    print('-- BEGIN TRADE with new price {} and maxVolume of {} --'.format(finalPrice,maxVolume))
    for order in ordersSorted:
        if (order['type'] == 'sell' and float(order['price']) <= finalPrice and order['idStock'] == idStock):
            item = dynamo.orders.get_item(id=order['id'])
            itAmount = item['amount']

            if (itAmount + sellAmount) <= maxVolume:
                item['amount'] = 0
                sellAmount += itAmount
                if item['txhistory'] == None:
                    item['txhistory'] = [[str(itAmount),str(finalPrice)],]
                else:
                    item['txhistory'].append([str(itAmount),str(finalPrice)])
                item.save()
                print('traded: order {} -- stock {} -- type {} -- amount {} -- sellAmount {}'.format(item['id'],item['idStock'],item['type'],itAmount,sellAmount))
            elif (itAmount + sellAmount) > maxVolume and sellAmount < maxVolume:
                partAmount = maxVolume - sellAmount
                item['amount'] -= partAmount
                sellAmount += partAmount
                if item['txhistory'] == None:
                    item['txhistory'] = [[str(partAmount),str(finalPrice)],]
                else:
                    item['txhistory'].append([str(partAmount),str(finalPrice)])
                item.save()
                print('traded: order {} -- stock {} -- type {} -- amount {} -- sellAmount {}'.format(item['id'],item['idStock'],item['type'],partAmount,sellAmount))

        elif (order['type'] == 'buy' and float(order['price']) >= finalPrice and order['idStock'] == idStock):
            item = dynamo.orders.get_item(id=order['id'])
            itAmount = item['amount']

            if (itAmount + buyAmount) <= maxVolume:
                item['amount'] = 0
                buyAmount += itAmount
                if item['txhistory'] == None:
                    item['txhistory'] = [[str(itAmount),str(finalPrice)],]
                else:
                    item['txhistory'].append([str(itAmount),str(finalPrice)])
                item.save()
                print('traded: order {} -- stock {} -- type {} -- amount {} -- buyAmount {}'.format(item['id'],item['idStock'],item['type'],itAmount,buyAmount))
            elif (itAmount + buyAmount) > maxVolume and buyAmount < maxVolume:
                partAmount = maxVolume - sellAmount
                item['amount'] -= partAmount
                buyAmount += partAmount
                if item['txhistory'] == None:
                    item['txhistory'] = [[str(partAmount),str(finalPrice)],]
                else:
                    item['txhistory'].append([str(partAmount),str(finalPrice)])

                item.save()
                print('traded: order {} -- stock {} -- type {} -- amount {} -- buyAmount {}'.format(item['id'],item['idStock'],item['type'],partAmount,buyAmount))

    return finalPrice

def getkey(custom):
    return custom['timestamp']

if __name__ == "__main__":
    app.run(host='0.0.0.0')
